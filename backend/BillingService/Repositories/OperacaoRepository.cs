using BillingService.Data;
using BillingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Repositories;

public class OperacaoRepository
{
    private readonly BillingDbContext _context;

    public OperacaoRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Operacao>> GetByUserIdAsync(Guid userId, int? ano, int? mes, bool? isAtiva)
    {
        var operacoesIds = await _context.UsuarioOperacoes
            .Where(uo => uo.UserId == userId)
            .Select(uo => uo.OperacaoId)
            .ToListAsync();

        var query = _context.Operacoes
            .Where(op => operacoesIds.Contains(op.Id))
            .AsQueryable();

        // Aplica filtros na Operação
        if (isAtiva.HasValue) query = query.Where(op => op.IsAtiva == isAtiva.Value);
        if (ano.HasValue) query = query.Where(op => op.DataInicio.Year == ano.Value);
        if (mes.HasValue) query = query.Where(op => op.DataInicio.Month == mes.Value);

        // --- CORREÇÃO AQUI ---
        // Carrega os faturamentos, aplicando os mesmos filtros de data
        // Isso garante que a Projeção seja calculada corretamente
        return await query.Select(op => new Operacao
        {
            // Copia todas as propriedades da operação
            Id = op.Id,
            Nome = op.Nome,
            Descricao = op.Descricao,
            Endereco = op.Endereco,
            MetaMensal = op.MetaMensal,
            ProjecaoFaturamento = op.ProjecaoFaturamento,
            DataInicio = op.DataInicio,
            DataFim = op.DataFim,
            IsAtiva = op.IsAtiva,
            UserId = op.UserId,
            // Filtra a lista de faturamentos incluída
            Faturamentos = op.Faturamentos
                .Where(f => (!ano.HasValue || f.Data.Year == ano.Value) &&
                            (!mes.HasValue || f.Data.Month == mes.Value))
                .ToList()
        }).ToListAsync();
    }

    // O método GetAllAsync também se beneficia da mesma correção
    public async Task<IEnumerable<Operacao>> GetAllAsync(int? ano, int? mes, bool? isAtiva)
    {
        var query = _context.Operacoes.AsQueryable();

        if (isAtiva.HasValue) query = query.Where(op => op.IsAtiva == isAtiva.Value);
        if (ano.HasValue) query = query.Where(op => op.DataInicio.Year == ano.Value);
        if (mes.HasValue) query = query.Where(op => op.DataInicio.Month == mes.Value);

        return await query.Select(op => new Operacao
        {
            Id = op.Id,
            Nome = op.Nome,
            Descricao = op.Descricao,
            Endereco = op.Endereco,
            MetaMensal = op.MetaMensal,
            ProjecaoFaturamento = op.ProjecaoFaturamento,
            DataInicio = op.DataInicio,
            DataFim = op.DataFim,
            IsAtiva = op.IsAtiva,
            UserId = op.UserId,
            Faturamentos = op.Faturamentos
                .Where(f => (!ano.HasValue || f.Data.Year == ano.Value) &&
                            (!mes.HasValue || f.Data.Month == mes.Value))
                .ToList()
        }).ToListAsync();
    }

    public async Task<Operacao?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        var temAcesso = await _context.UsuarioOperacoes.AnyAsync(uo => uo.UserId == userId && uo.OperacaoId == id);
        if (!temAcesso)
        {
            return null;
        }

        // Esta consulta específica não precisa do filtro de data, pois busca por ID
        return await _context.Operacoes
            .Include(op => op.Faturamentos)
            .FirstOrDefaultAsync(op => op.Id == id);
    }

    // --- MÉTODOS RESTANTES ESTÃO CORRETOS ---
    public async Task AddAsync(Operacao operacao)
    {
        await _context.Operacoes.AddAsync(operacao);
    }

    public async Task AddUsuarioOperacaoLinkAsync(UsuarioOperacao vinculo)
    {
        await _context.UsuarioOperacoes.AddAsync(vinculo);
    }

    public void Update(Operacao operacao)
    {
        _context.Operacoes.Update(operacao);
    }

    public void Remove(Operacao operacao)
    {
        _context.Operacoes.Remove(operacao);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao)
    {
        // Encontra a operação diretamente pelo ID
        var operacao = await _context.Operacoes.FindAsync(id);
        if (operacao == null)
        {
            return false; // Retorna falso se não encontrar
        }

        // Atualiza o campo e salva no banco
        operacao.ProjecaoFaturamento = projecao;
        await _context.SaveChangesAsync();

        return true; // Retorna verdadeiro indicando sucesso
    }
}