// Caminho: backend/BillingService/Services/FaturamentoDiarioService.cs
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BillingService.Services
{
    public class FaturamentoDiarioService : IFaturamentoDiarioService
    {
        private readonly IFaturamentoDiarioRepository _repository;
        private readonly IUnidadeRepository _unidadeRepository;

        public FaturamentoDiarioService(IFaturamentoDiarioRepository repository, IUnidadeRepository unidadeRepository)
        {
            _repository = repository;
            _unidadeRepository = unidadeRepository;
        }

        // Método para o "Líder" (Operador)
        public async Task<(FaturamentoDiarioResponseDto? dto, string? errorMessage)> SubmeterFechamentoAsync(Guid unidadeId, FaturamentoDiarioCreateDto dto, Guid userId, Guid tenantId)
        {
            // Valida se a Unidade existe
            var unidade = await _unidadeRepository.GetByIdAsync(unidadeId, tenantId);
            if (unidade == null)
            {
                return (null, "Unidade não encontrada.");
            }
            
            // Valida se já existe um fechamento para este dia
            var existente = await _repository.GetByUnidadeAndDateAsync(unidadeId, dto.Data, tenantId);
            if (existente != null)
            {
                return (null, "Já existe um fechamento submetido para esta data.");
            }

            var novoFechamento = new FaturamentoDiario
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UnidadeId = unidadeId,
                Data = dto.Data,
                Status = RegistroStatus.Pendente, // Inicia como Pendente
                FundoDeCaixa = dto.FundoDeCaixa,
                Observacoes = dto.Observacoes,
                // Campos do Supervisor são nulos no início
                ValorAtm = null,
                ValorBoletosMensalistas = null
            };

            await _repository.AddAsync(novoFechamento);
            await _repository.SaveChangesAsync();

            return (MapToResponseDto(novoFechamento), null);
        }

        // Método para o "Supervisor"
        public async Task<(FaturamentoDiarioResponseDto? dto, string? errorMessage)> RevisarFechamentoAsync(Guid faturamentoDiarioId, FaturamentoDiarioSupervisorUpdateDto dto, Guid supervisorId, Guid tenantId)
        {
            var fechamento = await _repository.GetByIdAsync(faturamentoDiarioId, tenantId);
            if (fechamento == null)
            {
                return (null, "Fechamento não encontrado.");
            }

            if (fechamento.Status == RegistroStatus.Aprovado && dto.Status == RegistroStatus.Pendente)
            {
                return (null, "Não é permitido reverter um fechamento Aprovado para Pendente.");
            }

            // Atualiza os campos (tanto do operador quanto do supervisor)
            fechamento.Status = dto.Status;
            fechamento.FundoDeCaixa = dto.FundoDeCaixa;
            fechamento.Observacoes = dto.Observacoes;
            fechamento.ValorAtm = dto.ValorAtm;
            fechamento.ValorBoletosMensalistas = dto.ValorBoletosMensalistas;

            _repository.Update(fechamento);
            await _repository.SaveChangesAsync();

            return (MapToResponseDto(fechamento), null);
        }

        public async Task<FaturamentoDiarioResponseDto?> GetFechamentoByIdAsync(Guid id, Guid tenantId)
        {
            var fechamento = await _repository.GetByIdAsync(id, tenantId);
            return fechamento == null ? null : MapToResponseDto(fechamento);
        }

        public async Task<IEnumerable<FaturamentoDiarioResponseDto>> GetFechamentosPorUnidadeAsync(Guid unidadeId, Guid tenantId)
        {
            var fechamentos = await _repository.ListByUnidadeAsync(unidadeId, tenantId);
            return fechamentos.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<FaturamentoDiarioResponseDto>> GetFechamentosPendentesAsync(Guid tenantId)
        {
            var fechamentos = await _repository.ListByStatusAsync(RegistroStatus.Pendente, tenantId);
            // Mapeia para um DTO que inclui o nome da Unidade
            return fechamentos.Select(fd => new FaturamentoDiarioResponseDto
            {
                Id = fd.Id,
                UnidadeId = fd.UnidadeId,
                // UnidadeName = fd.Unidade.Nome, // (Opcional, se o frontend precisar)
                Data = fd.Data,
                Status = fd.Status.ToString(),
                ValorTotalParciais = fd.FaturamentosParciais.Sum(fp => fp.Valor)
            });
        }

        // --- Método Helper Privado ---

        /// <summary>
        /// Mapeia a entidade FaturamentoDiario para o DTO de Resposta,
        /// calculando o valor total dos parciais.
        /// </summary>
        private FaturamentoDiarioResponseDto MapToResponseDto(FaturamentoDiario fechamento)
        {
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
                // Calcula o total dos "itens" (Faturamentos Parciais)
                ValorTotalParciais = fechamento.FaturamentosParciais?.Sum(fp => fp.Valor) ?? 0
            };
        }
    }
}