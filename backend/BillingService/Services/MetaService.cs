using BillingService.DTO;
using BillingService.Models;
using BillingService.Repositories;

namespace BillingService.Services;

public class MetaService
{
    private readonly MetaRepository _repository;

    public MetaService(MetaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Meta?> GetMetaAsync(Guid userId, int mes, int ano)
    {
        return await _repository.GetByUserAndPeriodAsync(userId, mes, ano);
    }

    public async Task<Meta> SetMetaAsync(MetaDto metaDto, Guid userId)
    {
        var metaExistente = await _repository.GetByUserAndPeriodAsync(userId, metaDto.Mes, metaDto.Ano);

        if (metaExistente != null)
        {
            // Se existe, atualiza o valor
            metaExistente.ValorAlvo = metaDto.ValorAlvo;
            _repository.Update(metaExistente);
            await _repository.SaveChangesAsync();
            return metaExistente;
        }
        else
        {
            // Se não existe, cria uma nova
            var novaMeta = new Meta
            {
                Id = Guid.NewGuid(),
                Mes = metaDto.Mes,
                Ano = metaDto.Ano,
                ValorAlvo = metaDto.ValorAlvo,
                UserId = userId
            };
            await _repository.AddAsync(novaMeta);
            await _repository.SaveChangesAsync();
            return novaMeta;
        }
    }
}