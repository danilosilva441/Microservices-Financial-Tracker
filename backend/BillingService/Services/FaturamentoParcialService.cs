using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using BillingService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Exceptions;
using BillingService.Services.Exceptions;

namespace BillingService.Services
{
    public class FaturamentoParcialService : IFaturamentoParcialService
    {
        private readonly IFaturamentoParcialRepository _repository;
        private readonly IUnidadeRepository _unidadeRepository;
        private readonly BillingDbContext _context;
        private readonly ILogger<FaturamentoParcialService> _logger;

        public FaturamentoParcialService(
            IFaturamentoParcialRepository repository,
            IUnidadeRepository unidadeRepository,
            BillingDbContext context,
            ILogger<FaturamentoParcialService> logger)
        {
            _repository = repository;
            _unidadeRepository = unidadeRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<FaturamentoParcial> AddFaturamentoAsync(
            Guid unidadeId, FaturamentoParcialCreateDto dto, Guid userId, Guid tenantId)
        {
            try
            {
                // Validação do DTO
                ValidateFaturamentoCreateDto(dto);

                // Verifica acesso à unidade
                await ValidateUserAccessAsync(unidadeId, userId, tenantId);

                // Encontra ou cria o FaturamentoDiario
                var dataDoFechamento = DateOnly.FromDateTime(dto.HoraInicio.Date);
                var faturamentoDiario = await GetOrCreateFaturamentoDiarioAsync(unidadeId, dataDoFechamento, tenantId);

                // Verifica sobreposição
                await ValidateNoOverlapAsync(faturamentoDiario.Id, tenantId, dto.HoraInicio, dto.HoraFim);

                // Cria o faturamento parcial
                var novoFaturamento = CreateFaturamentoParcialEntity(dto, faturamentoDiario.Id, tenantId);

                await _repository.AddAsync(novoFaturamento);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Faturamento parcial criado com sucesso: {FaturamentoId}", novoFaturamento.Id);
                return novoFaturamento;
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar faturamento parcial para unidade {UnidadeId}", unidadeId);
                throw new FaturamentoServiceException("Erro ao adicionar faturamento parcial", ex);
            }
        }

        public async Task UpdateFaturamentoAsync(
            Guid unidadeId, Guid faturamentoId, FaturamentoParcialUpdateDto dto, Guid userId, Guid tenantId)
        {
            try
            {
                // Validação do DTO
                ValidateFaturamentoUpdateDto(dto);

                // Verifica acesso à unidade
                await ValidateUserAccessAsync(unidadeId, userId, tenantId);

                var faturamentoExistente = await GetFaturamentoParcialAsync(faturamentoId, tenantId);
                await ValidateFaturamentoBelongsToUnidadeAsync(faturamentoExistente, unidadeId, tenantId);

                // Verifica sobreposição (excluindo o próprio registro)
                await ValidateNoOverlapAsync(faturamentoExistente.FaturamentoDiarioId, tenantId,
                    dto.HoraInicio, dto.HoraFim, faturamentoId);

                UpdateFaturamentoFromDto(faturamentoExistente, dto);
                _repository.Update(faturamentoExistente);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Faturamento parcial atualizado com sucesso: {FaturamentoId}", faturamentoId);
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar faturamento parcial {FaturamentoId}", faturamentoId);
                throw new FaturamentoServiceException("Erro ao atualizar faturamento parcial", ex);
            }
        }

        public async Task DeleteFaturamentoAsync(Guid unidadeId, Guid faturamentoId, Guid userId, Guid tenantId)
        {
            try
            {
                // Verifica acesso à unidade
                await ValidateUserAccessAsync(unidadeId, userId, tenantId);

                var faturamentoExistente = await GetFaturamentoParcialAsync(faturamentoId, tenantId);
                await ValidateFaturamentoBelongsToUnidadeAsync(faturamentoExistente, unidadeId, tenantId);

                _repository.Remove(faturamentoExistente);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Faturamento parcial excluído com sucesso: {FaturamentoId}", faturamentoId);
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir faturamento parcial {FaturamentoId}", faturamentoId);
                throw new FaturamentoServiceException("Erro ao excluir faturamento parcial", ex);
            }
        }

        public async Task DeactivateFaturamentoAsync(Guid unidadeId, Guid faturamentoId, Guid userId, Guid tenantId)
        {
            try
            {
                // Verifica acesso à unidade
                await ValidateUserAccessAsync(unidadeId, userId, tenantId);

                var faturamentoExistente = await GetFaturamentoParcialAsync(faturamentoId, tenantId);
                await ValidateFaturamentoBelongsToUnidadeAsync(faturamentoExistente, unidadeId, tenantId);

                faturamentoExistente.IsAtivo = false;
                _repository.Update(faturamentoExistente);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Faturamento parcial desativado com sucesso: {FaturamentoId}", faturamentoId);
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar faturamento parcial {FaturamentoId}", faturamentoId);
                throw new FaturamentoServiceException("Erro ao desativar faturamento parcial", ex);
            }
        }

        // Método auxiliar adicional para buscar faturamentos
        public async Task<IEnumerable<FaturamentoParcial>> GetFaturamentosPorUnidadeEDataAsync(
            Guid unidadeId, DateOnly data, Guid tenantId)
        {
            try
            {
                _logger.LogDebug("Buscando faturamentos parciais da unidade {UnidadeId} na data {Data}",
                    unidadeId, data);

                // Busca o faturamento diário primeiro
                var faturamentoDiario = await _context.FaturamentosDiarios
                    .FirstOrDefaultAsync(fd => fd.UnidadeId == unidadeId && fd.Data == data && fd.TenantId == tenantId);

                if (faturamentoDiario == null)
                {
                    return Enumerable.Empty<FaturamentoParcial>();
                }

                // Busca os faturamentos parciais relacionados
                return await _context.FaturamentosParciais
                    .Where(fp => fp.FaturamentoDiarioId == faturamentoDiario.Id && fp.TenantId == tenantId && fp.IsAtivo)
                    .OrderBy(fp => fp.HoraInicio)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar faturamentos parciais da unidade {UnidadeId}", unidadeId);
                throw new FaturamentoServiceException("Erro ao buscar faturamentos parciais", ex);
            }
        }

        #region Private Methods

        private async Task ValidateUserAccessAsync(Guid unidadeId, Guid userId, Guid tenantId)
        {
            if (!await _repository.UserHasAccessToUnidadeAsync(unidadeId, userId, tenantId))
            {
                throw new UnidadeAccessDeniedException(unidadeId);
            }
        }

        private async Task<FaturamentoParcial> GetFaturamentoParcialAsync(Guid faturamentoId, Guid tenantId)
        {
            var faturamento = await _repository.GetByIdAsync(faturamentoId, tenantId);
            if (faturamento == null)
            {
                throw new FaturamentoParcialNotFoundException(faturamentoId);
            }
            return faturamento;
        }

        private async Task ValidateFaturamentoBelongsToUnidadeAsync(
            FaturamentoParcial faturamento, Guid unidadeId, Guid tenantId)
        {
            var faturamentoDiario = await _context.FaturamentosDiarios
                .FirstOrDefaultAsync(fd => fd.Id == faturamento.FaturamentoDiarioId && fd.TenantId == tenantId);

            if (faturamentoDiario == null || faturamentoDiario.UnidadeId != unidadeId)
            {
                throw new UnidadeAccessDeniedException(unidadeId);
            }
        }

        private async Task ValidateNoOverlapAsync(
            Guid faturamentoDiarioId, Guid tenantId, DateTime inicio, DateTime fim, Guid? excludeFaturamentoId = null)
        {
            if (await _repository.CheckForOverlappingFaturamentoAsync(
                faturamentoDiarioId, tenantId, inicio, fim, excludeFaturamentoId))
            {
                throw new FaturamentoOverlapException();
            }
        }

        private static void ValidateFaturamentoCreateDto(FaturamentoParcialCreateDto dto)
        {
            if (dto == null)
                throw new FaturamentoDtoNullException();

            if (dto.Valor <= 0)
                throw new BusinessException("VALOR_INVALIDO", ErrorCodes.RequiredField, "Valor deve ser maior que zero");

            if (dto.HoraInicio >= dto.HoraFim)
                throw new InvalidFaturamentoTimeException("Hora fim deve ser posterior à hora início");

            if (dto.HoraFim > DateTime.UtcNow)
                throw new InvalidFaturamentoTimeException("Hora fim não pode ser futura");

            if (string.IsNullOrWhiteSpace(dto.Origem))
                throw new BusinessException("INVALID_ORIGEM", ErrorCodes.InvalidDateRange, "Hora fim deve ser posterior à hora início");
        }

        private static void ValidateFaturamentoUpdateDto(FaturamentoParcialUpdateDto dto)
        {
            if (dto == null)
                throw new BusinessException("DTO_NULL", ErrorCodes.RequiredField, "DTO não pode ser nulo");

            if (dto.Valor <= 0)
                throw new BusinessException("VALOR_INVALIDO", ErrorCodes.RequiredField, "Valor deve ser maior que zero");

            if (dto.HoraInicio >= dto.HoraFim)
                throw new InvalidFaturamentoTimeException("Hora fim deve ser posterior à hora início");

            if (dto.HoraFim > DateTime.UtcNow)
                throw new InvalidFaturamentoTimeException("Hora fim não pode ser futura");

            if (string.IsNullOrWhiteSpace(dto.Origem))
                throw new BusinessException("INVALID_ORIGEM", ErrorCodes.InvalidDateRange, "Origem não pode ser nula ou vazia");
        }

        private async Task<FaturamentoDiario> GetOrCreateFaturamentoDiarioAsync(
            Guid unidadeId, DateOnly data, Guid tenantId)
        {
            var faturamentoDiario = await _context.FaturamentosDiarios
                .FirstOrDefaultAsync(fd => fd.UnidadeId == unidadeId && fd.Data == data && fd.TenantId == tenantId);

            if (faturamentoDiario == null)
            {
                faturamentoDiario = new FaturamentoDiario
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    UnidadeId = unidadeId,
                    Data = data,
                    Status = RegistroStatus.Pendente,
                    FundoDeCaixa = 0,
                    Observacoes = "Criado automaticamente para faturamentos parciais"
                };
                await _context.FaturamentosDiarios.AddAsync(faturamentoDiario);
            }

            return faturamentoDiario;
        }

        private static FaturamentoParcial CreateFaturamentoParcialEntity(
            FaturamentoParcialCreateDto dto, Guid faturamentoDiarioId, Guid tenantId)
        {
            return new FaturamentoParcial
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FaturamentoDiarioId = faturamentoDiarioId,
                Valor = dto.Valor,
                HoraInicio = dto.HoraInicio,
                HoraFim = dto.HoraFim,
                MetodoPagamentoId = dto.MetodoPagamentoId,
                Origem = dto.Origem,
                IsAtivo = true
            };
        }

        private static void UpdateFaturamentoFromDto(FaturamentoParcial faturamento, FaturamentoParcialUpdateDto dto)
        {
            faturamento.Valor = dto.Valor;
            faturamento.HoraInicio = dto.HoraInicio;
            faturamento.HoraFim = dto.HoraFim;
            faturamento.MetodoPagamentoId = dto.MetodoPagamentoId;
            faturamento.Origem = dto.Origem;
        }

        #endregion
    }
}