using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using SharedKernel;
using SharedKernel.Exceptions;
using BillingService.Services.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Security.Cryptography;
using BillingService.Data;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Services
{
    public class FaturamentoDiarioService : IFaturamentoDiarioService
    {
        private readonly IFaturamentoDiarioRepository _repository;
        private readonly IUnidadeRepository _unidadeRepository;
        private readonly ILogger<FaturamentoDiarioService> _logger;
        private readonly BillingDbContext _context;

        public FaturamentoDiarioService(
            IFaturamentoDiarioRepository repository,
            IUnidadeRepository unidadeRepository,
            ILogger<FaturamentoDiarioService> logger,
            BillingDbContext context)
        {
            _repository = repository;
            _unidadeRepository = unidadeRepository;
            _logger = logger;
            _context = context;
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
                StatusCaixa = fechamento.StatusCaixa.ToString(), // Novo
                FundoDeCaixa = fechamento.FundoDeCaixa,
                Observacoes = fechamento.Observacoes,
                ValorAtm = fechamento.ValorAtm,
                ValorBoletosMensalistas = fechamento.ValorBoletosMensalistas,
                ValorTotalCalculado = fechamento.ValorTotalCalculado,
                ValorConferido = fechamento.ValorConferido,
                Diferenca = fechamento.Diferenca,
                HashAssinatura = fechamento.HashAssinatura,
                DataFechamento = fechamento.DataFechamento,
                DataConferencia = fechamento.DataConferencia,
                ObservacoesConferencia = fechamento.ObservacoesConferencia,
                FechadoPorUserId = fechamento.FechadoPorUserId,
                ConferidoPorUserId = fechamento.ConferidoPorUserId,
                ValorTotalParciais = CalculateValorTotalParciais(fechamento.FaturamentosParciais)
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

        public async Task<IEnumerable<FaturamentoDiario>> GetFechamentosPorDataAsync(
           Guid unidadeId,
           DateOnly dataInicio,
           DateOnly dataFim,
           Guid tenantId)
        {
            try
            {
                _logger.LogDebug(
                    "Buscando fechamentos por intervalo de datas - Unidade: {UnidadeId}, Tenant: {TenantId}, Data Início: {DataInicio}, Data Fim: {DataFim}",
                    unidadeId, tenantId, dataInicio, dataFim);

                // Validações
                if (dataInicio > dataFim)
                {
                    throw new ArgumentException("A data de início não pode ser maior que a data de fim.");
                }

                // Limita o intervalo máximo (opcional)
                var maxDias = 90;
                if ((dataFim.DayNumber - dataInicio.DayNumber) > maxDias)
                {
                    throw new ArgumentException($"O intervalo máximo permitido é de {maxDias} dias.");
                }

                // Use o método correto do repository
                return await _repository.GetByDateRangeAsync(tenantId, unidadeId, dataInicio, dataFim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fechamentos por intervalo de datas");
                throw;
            }
        }

        #region Fechamento de Caixa

        /// <summary>
        /// Fecha o caixa do dia (Operador)
        /// </summary>
        public async Task<ResultadoFechamentoDto> FecharCaixaAsync(
            Guid faturamentoId,
            decimal valorConferido,
            string? observacoes,
            Guid usuarioId)
        {
            try
            {
                _logger.LogInformation("Iniciando fechamento de caixa {FaturamentoId}", faturamentoId);

                // Busca o faturamento com todos os relacionamentos necessários
                var faturamento = await _context.FaturamentosDiarios
                    .Include(f => f.FaturamentosParciais)
                    .FirstOrDefaultAsync(f => f.Id == faturamentoId);

                if (faturamento == null)
                {
                    _logger.LogWarning("Faturamento não encontrado: {FaturamentoId}", faturamentoId);
                    return new ResultadoFechamentoDto
                    {
                        Sucesso = false,
                        Mensagem = "Caixa não encontrado."
                    };
                }

                // Verifica se o caixa já está fechado
                if (faturamento.StatusCaixa != StatusCaixa.Aberto)
                {
                    _logger.LogWarning("Tentativa de fechar caixa já fechado: {FaturamentoId}", faturamentoId);
                    return new ResultadoFechamentoDto
                    {
                        Sucesso = false,
                        Mensagem = "Este caixa já está fechado."
                    };
                }

                // --- MUDANÇA PRINCIPAL AQUI ---
                // 1. Calcula o total (Adicionei o filtro IsAtivo para segurança)
                decimal valorTotalParciais = faturamento.FaturamentosParciais?
                    .Where(fp => fp.IsAtivo) // Garante que só somamos os ativos
                    .Sum(fp => fp.Valor) ?? 0m;

                decimal diferenca = valorConferido - valorTotalParciais;

                // 2. Atualiza o faturamento
                // Preenchemos a NOVA PROPRIEDADE para o AnalysisService ler rápido
                faturamento.ValorTotal = valorTotalParciais;

                // Mantemos a sua propriedade antiga se você usa para exibição interna
                faturamento.ValorTotalCalculado = valorTotalParciais;

                faturamento.ValorConferido = valorConferido;
                faturamento.Diferenca = diferenca;
                faturamento.Observacoes = observacoes;
                faturamento.StatusCaixa = StatusCaixa.Fechado;
                faturamento.FechadoPorUserId = usuarioId;
                faturamento.DataFechamento = DateTime.UtcNow;

                // Gera hash de segurança
                faturamento.HashAssinatura = GerarHashAssinatura(faturamento, usuarioId);

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Caixa fechado com sucesso: {FaturamentoId}. Valor Total: {Total}. Diferença: {Diferenca}",
                    faturamentoId, valorTotalParciais, diferenca);

                return new ResultadoFechamentoDto
                {
                    Sucesso = true,
                    Mensagem = "Caixa fechado com sucesso.",
                    Hash = faturamento.HashAssinatura,
                    Diferenca = diferenca,
                    ValorTotalCalculado = valorTotalParciais,
                    ValorConferido = valorConferido
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar caixa {FaturamentoId}", faturamentoId);
                throw new FaturamentoServiceException("Erro ao fechar caixa", ex);
            }
        }

        /// <summary>
        /// Registra conferência do caixa (Supervisor/Gerente)
        /// </summary>
        public async Task<bool> RegistrarConferenciaAsync(
            Guid faturamentoId,
            bool aprovado,
            string? observacoes,
            Guid usuarioId)
        {
            try
            {
                _logger.LogInformation("Iniciando conferência de caixa {FaturamentoId}", faturamentoId);

                var faturamento = await _context.FaturamentosDiarios
                    .FirstOrDefaultAsync(f => f.Id == faturamentoId);

                if (faturamento == null)
                {
                    _logger.LogWarning("Faturamento não encontrado para conferência: {FaturamentoId}", faturamentoId);
                    throw new KeyNotFoundException("Caixa não encontrado.");
                }

                // Verifica se o caixa está fechado para poder conferir
                if (faturamento.StatusCaixa != StatusCaixa.Fechado)
                {
                    _logger.LogWarning("Tentativa de conferir caixa não fechado: {FaturamentoId}", faturamentoId);
                    throw new InvalidOperationException("O caixa precisa estar fechado para ser conferido.");
                }

                // Atualiza status
                faturamento.StatusCaixa = aprovado ? StatusCaixa.Conferido : StatusCaixa.Fechado;
                faturamento.ConferidoPorUserId = usuarioId;
                faturamento.DataConferencia = DateTime.UtcNow;
                faturamento.ObservacoesConferencia = observacoes;

                // Se aprovado, também atualiza o status geral
                if (aprovado)
                {
                    faturamento.Status = RegistroStatus.Aprovado;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Conferência registrada com sucesso: {FaturamentoId}. Aprovado: {Aprovado}",
                    faturamentoId, aprovado);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar conferência do caixa {FaturamentoId}", faturamentoId);
                throw new FaturamentoServiceException("Erro ao registrar conferência", ex);
            }
        }

        /// <summary>
        /// Reabre um caixa fechado (Apenas Admin)
        /// </summary>
        public async Task<bool> ReabrirCaixaAsync(
            Guid faturamentoId,
            string motivo,
            Guid usuarioId)
        {
            try
            {
                _logger.LogInformation("Iniciando reabertura de caixa {FaturamentoId}", faturamentoId);

                var faturamento = await _context.FaturamentosDiarios
                    .FirstOrDefaultAsync(f => f.Id == faturamentoId);

                if (faturamento == null)
                {
                    _logger.LogWarning("Faturamento não encontrado para reabertura: {FaturamentoId}", faturamentoId);
                    throw new KeyNotFoundException("Caixa não encontrado.");
                }

                // Reset dos campos de fechamento
                faturamento.StatusCaixa = StatusCaixa.Aberto;
                faturamento.ValorTotalCalculado = null;
                faturamento.ValorConferido = null;
                faturamento.Diferenca = null;
                faturamento.HashAssinatura = null;
                faturamento.DataFechamento = null;
                faturamento.FechadoPorUserId = null;
                faturamento.DataConferencia = null;
                faturamento.ConferidoPorUserId = null;
                faturamento.ObservacoesConferencia = null;

                // Adiciona o motivo da reabertura às observações
                if (!string.IsNullOrEmpty(motivo))
                {
                    faturamento.Observacoes += $"\n[REABERTURA {DateTime.UtcNow:dd/MM/yyyy HH:mm}] {motivo}";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Caixa reaberto com sucesso: {FaturamentoId}", faturamentoId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reabrir caixa {FaturamentoId}", faturamentoId);
                throw new FaturamentoServiceException("Erro ao reabrir caixa", ex);
            }
        }

        /// <summary>
        /// Gera hash de segurança para o fechamento
        /// </summary>
        private string GerarHashAssinatura(FaturamentoDiario faturamento, Guid usuarioId)
        {
            var dadosParaHash = $"{faturamento.Id}|" +
                               $"{faturamento.ValorTotalCalculado}|" +
                               $"{faturamento.DataFechamento:yyyy-MM-dd HH:mm:ss}|" +
                               $"{usuarioId}|" +
                               $"{faturamento.UnidadeId}";

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(dadosParaHash);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower()[..16]; // Primeiros 16 caracteres
        }

        #endregion
    }
}
