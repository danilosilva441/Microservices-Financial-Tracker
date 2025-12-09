using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using SharedKernel;
using SharedKernel.Exceptions;
using BillingService.Services.Exceptions;
using Microsoft.Extensions.Logging;

namespace BillingService.Services
{
    public class FaturamentoDiarioService : IFaturamentoDiarioService
    {
        private readonly IFaturamentoDiarioRepository _repository;
        private readonly IUnidadeRepository _unidadeRepository;
        private readonly ILogger<FaturamentoDiarioService> _logger;

        public FaturamentoDiarioService(
            IFaturamentoDiarioRepository repository, 
            IUnidadeRepository unidadeRepository,
            ILogger<FaturamentoDiarioService> logger)
        {
            _repository = repository;
            _unidadeRepository = unidadeRepository;
            _logger = logger;
        }

        public async Task<(FaturamentoDiarioResponseDto? dto, string? errorMessage)> SubmeterFechamentoAsync(
            Guid unidadeId, 
            FaturamentoDiarioCreateDto dto, 
            Guid userId, 
            Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Iniciando submissão de fechamento para unidade {UnidadeId}", unidadeId);

                // Validação inicial do DTO
                var validationError = ValidateFechamentoCreateDto(dto);
                if (validationError != null)
                {
                    _logger.LogWarning("Validação falhou para submissão de fechamento: {Error}", validationError);
                    return (null, validationError);
                }

                // Valida se a Unidade existe
                var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
                if (unidade == null)
                {
                    _logger.LogWarning("Unidade não encontrada: {UnidadeId}", unidadeId);
                    return (null, ErrorMessages.UnidadeNotFound);
                }
                
                // Valida se já existe um fechamento para este dia
                var existente = await _repository.GetByUnidadeAndDateAsync(unidadeId, dto.Data, tenantId);
                if (existente != null)
                {
                    _logger.LogWarning("Fechamento já existe para unidade {UnidadeId} na data {Data}", 
                        unidadeId, dto.Data.ToString("yyyy-MM-dd"));
                    return (null, ErrorMessages.FechamentoJaExiste);
                }

                var novoFechamento = CreateFechamentoEntity(unidadeId, dto, tenantId);
                
                await _repository.AddAsync(novoFechamento);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Fechamento submetido com sucesso: {FechamentoId}", novoFechamento.Id);
                
                return (MapToResponseDto(novoFechamento), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao submeter fechamento para unidade {UnidadeId}", unidadeId);
                throw new FaturamentoServiceException("Erro ao submeter fechamento", ex);
            }
        }

        public async Task<(FaturamentoDiarioResponseDto? dto, string? errorMessage)> RevisarFechamentoAsync(
            Guid faturamentoDiarioId, 
            FaturamentoDiarioSupervisorUpdateDto dto, 
            Guid supervisorId, 
            Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Iniciando revisão de fechamento {FechamentoId} por supervisor {SupervisorId}", 
                    faturamentoDiarioId, supervisorId);

                // Validação do DTO
                var validationError = ValidateFechamentoSupervisorDto(dto);
                if (validationError != null)
                {
                    _logger.LogWarning("Validação falhou para revisão de fechamento: {Error}", validationError);
                    return (null, validationError);
                }

                var fechamento = await _repository.GetByIdAsync(faturamentoDiarioId, tenantId);
                if (fechamento == null)
                {
                    _logger.LogWarning("Fechamento não encontrado: {FechamentoId}", faturamentoDiarioId);
                    return (null, ErrorMessages.FechamentoNotFound);
                }

                // Valida transição de status
                var statusValidationError = ValidateStatusTransition(fechamento.Status, dto.Status);
                if (statusValidationError != null)
                {
                    _logger.LogWarning(
                        "Transição de status inválida: {StatusAtual} -> {NovoStatus}", 
                        fechamento.Status, dto.Status);
                    return (null, statusValidationError);
                }

                UpdateFechamentoFromSupervisorDto(fechamento, dto);
                
                _repository.Update(fechamento);
                await _repository.SaveChangesAsync();

                _logger.LogInformation(
                    "Fechamento {FechamentoId} revisado com sucesso. Novo status: {Status}", 
                    fechamento.Id, dto.Status);

                return (MapToResponseDto(fechamento), null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao revisar fechamento {FechamentoId}", faturamentoDiarioId);
                throw new FaturamentoServiceException("Erro ao revisar fechamento", ex);
            }
        }

        public async Task<FaturamentoDiarioResponseDto?> GetFechamentoByIdAsync(Guid id, Guid tenantId)
        {
            try
            {
                _logger.LogDebug("Buscando fechamento por ID: {FechamentoId}", id);

                var fechamento = await _repository.GetByIdAsync(id, tenantId);
                
                if (fechamento == null)
                {
                    _logger.LogDebug("Fechamento não encontrado: {FechamentoId}", id);
                    return null;
                }

                return MapToResponseDto(fechamento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fechamento por ID: {FechamentoId}", id);
                throw new FaturamentoServiceException("Erro ao buscar fechamento", ex);
            }
        }

        public async Task<IEnumerable<FaturamentoDiarioResponseDto>> GetFechamentosPorUnidadeAsync(
            Guid unidadeId, 
            Guid tenantId)
        {
            try
            {
                _logger.LogDebug("Buscando fechamentos da unidade {UnidadeId}", unidadeId);

                var fechamentos = await _repository.ListByUnidadeAsync(unidadeId, tenantId);
                return fechamentos.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fechamentos da unidade {UnidadeId}", unidadeId);
                throw new FaturamentoServiceException("Erro ao buscar fechamentos por unidade", ex);
            }
        }

        public async Task<IEnumerable<FaturamentoDiarioResponseDto>> GetFechamentosPendentesAsync(Guid tenantId)
        {
            try
            {
                _logger.LogDebug("Buscando fechamentos pendentes");

                var fechamentos = await _repository.ListByStatusAsync(RegistroStatus.Pendente, tenantId);
                
                return fechamentos.Select(fd => new FaturamentoDiarioResponseDto
                {
                    Id = fd.Id,
                    UnidadeId = fd.UnidadeId,
                    Data = fd.Data,
                    Status = fd.Status.ToString(),
                    FundoDeCaixa = fd.FundoDeCaixa,
                    Observacoes = fd.Observacoes,
                    ValorAtm = fd.ValorAtm,
                    ValorBoletosMensalistas = fd.ValorBoletosMensalistas,
                    ValorTotalParciais = CalculateValorTotalParciais(fd.FaturamentosParciais)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fechamentos pendentes");
                throw new FaturamentoServiceException("Erro ao buscar fechamentos pendentes", ex);
            }
        }

        // --- Métodos Helper Privados ---

        private static FaturamentoDiario CreateFechamentoEntity(
            Guid unidadeId, 
            FaturamentoDiarioCreateDto dto, 
            Guid tenantId)
        {
            return new FaturamentoDiario
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UnidadeId = unidadeId,
                Data = dto.Data,
                Status = RegistroStatus.Pendente,
                FundoDeCaixa = dto.FundoDeCaixa,
                Observacoes = dto.Observacoes,
                ValorAtm = null,
                ValorBoletosMensalistas = null
                // Removidos CriadoEm e AtualizadoEm pois não existem na entidade
            };
        }

        private static void UpdateFechamentoFromSupervisorDto(
            FaturamentoDiario fechamento, 
            FaturamentoDiarioSupervisorUpdateDto dto)
        {
            fechamento.Status = dto.Status;
            fechamento.FundoDeCaixa = dto.FundoDeCaixa;
            fechamento.Observacoes = dto.Observacoes;
            fechamento.ValorAtm = dto.ValorAtm;
            fechamento.ValorBoletosMensalistas = dto.ValorBoletosMensalistas;
            // Removido AtualizadoEm pois não existe na entidade
        }

        private static FaturamentoDiarioResponseDto MapToResponseDto(FaturamentoDiario fechamento)
        {
            if (fechamento == null) 
                throw new ArgumentNullException(nameof(fechamento));

            return new FaturamentoDiarioResponseDto
            {
                Id = fechamento.Id,
                UnidadeId = fechamento.UnidadeId,
                Data = fechamento.Data,
                Status = fechamento.Status.ToString(),
                FundoDeCaixa = fechamento.FundoDeCaixa,
                Observacoes = fechamento.Observacoes,
                ValorAtm = fechamento.ValorAtm,
                ValorBoletosMensalistas = fechamento.ValorBoletosMensalistas,
                ValorTotalParciais = CalculateValorTotalParciais(fechamento.FaturamentosParciais)
                // Removidos CriadoEm e AtualizadoEm pois não existem no DTO
            };
        }

        private static decimal CalculateValorTotalParciais(ICollection<FaturamentoParcial>? faturamentosParciais)
        {
            return faturamentosParciais?.Sum(fp => fp.Valor) ?? 0m;
        }

        // --- Validações ---

        private static string? ValidateFechamentoCreateDto(FaturamentoDiarioCreateDto dto)
        {
            if (dto == null)
                return "DTO não pode ser nulo";

            // Corrigido: Comparação entre DateOnly e DateOnly
            if (dto.Data > DateOnly.FromDateTime(DateTime.UtcNow))
                return "Data não pode ser futura";

            if (dto.FundoDeCaixa < 0)
                return "Fundo de caixa não pode ser negativo";

            return null;
        }

        private static string? ValidateFechamentoSupervisorDto(FaturamentoDiarioSupervisorUpdateDto dto)
        {
            if (dto == null)
                return "DTO não pode ser nulo";

            if (!Enum.IsDefined(typeof(RegistroStatus), dto.Status))
                return "Status inválido";

            if (dto.FundoDeCaixa < 0)
                return "Fundo de caixa não pode ser negativo";

            if (dto.ValorAtm < 0)
                return "Valor ATM não pode ser negativo";

            if (dto.ValorBoletosMensalistas < 0)
                return "Valor de boletos mensalistas não pode ser negativo";

            return null;
        }

        private static string? ValidateStatusTransition(RegistroStatus currentStatus, RegistroStatus newStatus)
        {
            // Transições permitidas:
            // Pendente -> Aprovado
            // Pendente -> Rejeitado
            // Aprovado/Rejeitado -> Pendente NÃO é permitido
            if (currentStatus == RegistroStatus.Aprovado && newStatus == RegistroStatus.Pendente)
            {
                return ErrorMessages.NoAlteredStatus;
            }

            if (currentStatus == RegistroStatus.Rejeitado && newStatus == RegistroStatus.Pendente)
            {
                return ErrorMessages.NoAlteredStatus;
            }

            return null;
        }
    }
}