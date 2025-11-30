using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces; // 1. IMPORTANTE
using BillingService.Data; // 2. IMPORTANTE (Para o DbContext)
using Microsoft.EntityFrameworkCore;
using SharedKernel; // 3. IMPORTANTE (Para o DbContext)

namespace BillingService.Services
{
    public class FaturamentoParcialService : IFaturamentoParcialService
    {
        private readonly IFaturamentoParcialRepository _repository;
        // 4. MUDANÇA: Injetamos o Repositório de Unidade e o DbContext
        private readonly IUnidadeRepository _unidadeRepository;
        private readonly BillingDbContext _context;

        public FaturamentoParcialService(
            IFaturamentoParcialRepository repository, 
            IUnidadeRepository unidadeRepository, 
            BillingDbContext context)
        {
            _repository = repository;
            _unidadeRepository = unidadeRepository;
            _context = context;
        }

        // 5. MUDANÇA (v2.0): Assinatura atualizada
        public async Task<(FaturamentoParcial? faturamento, string? errorMessage)> AddFaturamentoAsync(
            Guid unidadeId, FaturamentoParcialCreateDto dto, Guid userId, Guid tenantId)
        {
            // 6. MUDANÇA (v2.0): Usando o método v2.0 do repositório
            if (!await _repository.UserHasAccessToUnidadeAsync(unidadeId, userId, tenantId))
            {
                return (null, ErrorMessages.UnidadeNotFound + " ou " + ErrorMessages.UserNoPermission);
            }

            // 7. LÓGICA V2.0: Encontra ou Cria o FaturamentoDiario (o "cabeçalho")
            var dataDoFechamento = DateOnly.FromDateTime(dto.HoraInicio.Date);
            var faturamentoDiario = await _context.FaturamentosDiarios
                .FirstOrDefaultAsync(fd => fd.UnidadeId == unidadeId && fd.Data == dataDoFechamento);

            if (faturamentoDiario == null)
            {
                // Se não existir, cria um novo "cabeçalho"
                faturamentoDiario = new FaturamentoDiario
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    UnidadeId = unidadeId,
                    Data = dataDoFechamento,
                    Status = RegistroStatus.Pendente,
                    // TODO: Preencher FundoDeCaixa e Observacoes (Tarefa 3 do Roadmap)
                    FundoDeCaixa = 0, 
                    Observacoes = "Criado automaticamente."
                };
                await _context.FaturamentosDiarios.AddAsync(faturamentoDiario);
            }
            // (Não salvamos ainda, vamos salvar tudo no final)

            // 8. Validações v1.0 (agora no contexto do FaturamentoDiarioId)
            if (dto.HoraInicio >= dto.HoraFim)
            {
                return (null, ErrorMessages.InvalidDateRange);
            }
            
            // ... (outras validações de data) ...

            // 9. MUDANÇA (v2.0): Checa duplicidade usando o FaturamentoDiarioId
            if (await _repository.CheckForOverlappingFaturamentoAsync(faturamentoDiario.Id, tenantId, dto.HoraInicio, dto.HoraFim))
            {
                return (null, ErrorMessages.OverlappingFaturamento);
            }

            // 10. MUDANÇA (v2.0): Cria o modelo v2.0 (corrige CS0117)
            var novoFaturamento = new FaturamentoParcial
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId, 
                FaturamentoDiarioId = faturamentoDiario.Id, // <-- CORRIGIDO
                Valor = dto.Valor,
                HoraInicio = dto.HoraInicio, 
                HoraFim = dto.HoraFim, 
                MetodoPagamentoId = dto.MetodoPagamentoId, 
                Origem = dto.Origem,
                IsAtivo = true
            };

            await _repository.AddAsync(novoFaturamento);
            await _repository.SaveChangesAsync(); // Salva o FaturamentoDiario (se novo) e o FaturamentoParcial

            return (novoFaturamento, null);
        }
        
        // 11. MUDANÇA (v2.0): Assinatura atualizada
        public async Task<(bool success, string? errorMessage)> UpdateFaturamentoAsync(
            Guid unidadeId, Guid faturamentoId, FaturamentoParcialUpdateDto dto, Guid userId, Guid tenantId)
        {
            if (!await _repository.UserHasAccessToUnidadeAsync(unidadeId, userId, tenantId))
            {
                return (false, ErrorMessages.UnidadeNotFound + " ou " + ErrorMessages.UserNoPermission);
            }

            var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId, tenantId);
            
            // 12. MUDANÇA (v2.0): Corrige CS1061
            // Precisamos carregar o FaturamentoDiario para checar a UnidadeId
            var faturamentoDiario = await _context.FaturamentosDiarios
                .FirstOrDefaultAsync(fd => fd.Id == faturamentoExistente!.FaturamentoDiarioId);

            if (faturamentoExistente == null || faturamentoDiario == null || faturamentoDiario.UnidadeId != unidadeId)
            {
                return (false, ErrorMessages.GenericNotFound);
            }
            
            if (dto.HoraInicio >= dto.HoraFim)
            {
                return (false, ErrorMessages.InvalidDateRange  );
            }
            
            if (await _repository.CheckForOverlappingFaturamentoAsync(faturamentoDiario.Id, tenantId, dto.HoraInicio, dto.HoraFim, faturamentoId))
            {
                return (false, ErrorMessages.OverlappingFaturamento);
            }
            
            faturamentoExistente.Valor = dto.Valor;
            faturamentoExistente.HoraInicio = dto.HoraInicio;
            faturamentoExistente.HoraFim = dto.HoraFim;
            faturamentoExistente.MetodoPagamentoId = dto.MetodoPagamentoId;

            _repository.Update(faturamentoExistente);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        // 13. MUDANÇA (v2.0): Assinatura atualizada
        public async Task<(bool success, string? errorMessage)> DeleteFaturamentoAsync(
            Guid unidadeId, Guid faturamentoId, Guid userId, Guid tenantId)
        {
             if (!await _repository.UserHasAccessToUnidadeAsync(unidadeId, userId, tenantId))
            {
                return (false, ErrorMessages.UnidadeNotFound + " ou " + ErrorMessages.UserNoPermission);
            }

            var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId, tenantId);
            
            var faturamentoDiario = await _context.FaturamentosDiarios
                .FirstOrDefaultAsync(fd => fd.Id == faturamentoExistente!.FaturamentoDiarioId);

            if (faturamentoExistente == null || faturamentoDiario == null || faturamentoDiario.UnidadeId != unidadeId)
            {
                return (false, ErrorMessages.GenericNotFound);
            }

            _repository.Remove(faturamentoExistente);
            await _repository.SaveChangesAsync();

            return (true, null);
        }

        // 14. MUDANÇA (v2.0): Assinatura atualizada
        public async Task<(bool success, string? errorMessage)> DeactivateFaturamentoAsync(
            Guid unidadeId, Guid faturamentoId, Guid userId, Guid tenantId)
        {
             if (!await _repository.UserHasAccessToUnidadeAsync(unidadeId, userId, tenantId))
            {
                return (false, ErrorMessages.UnidadeNotFound + " ou " + ErrorMessages.UserNoPermission);
            }

            var faturamentoExistente = await _repository.GetByIdAsync(faturamentoId, tenantId);
            
            var faturamentoDiario = await _context.FaturamentosDiarios
                .FirstOrDefaultAsync(fd => fd.Id == faturamentoExistente!.FaturamentoDiarioId);

            if (faturamentoExistente == null || faturamentoDiario == null || faturamentoDiario.UnidadeId != unidadeId)
            {
                return (false, ErrorMessages.GenericNotFound);
            }

            faturamentoExistente.IsAtivo = false;
            _repository.Update(faturamentoExistente);
            await _repository.SaveChangesAsync();

            return (true, null);
        }
    }
}