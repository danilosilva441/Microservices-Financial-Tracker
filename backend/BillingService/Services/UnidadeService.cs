// Caminho: backend/BillingService/Services/UnidadeService.cs
using BillingService.DTOs;
using BillingService.Models;
using BillingService.Repositories.Interfaces;
using BillingService.Services.Interfaces;

namespace BillingService.Services;

public class UnidadeService : IUnidadeService
{
    private readonly IUnidadeRepository _repository;
    private readonly ILogger<UnidadeService> _logger; // Adicionando logging

    public UnidadeService(IUnidadeRepository repository, ILogger<UnidadeService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Unidade>> GetAllUnidadesByTenantAsync(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId não pode ser vazio", nameof(tenantId));
        }

        _logger.LogInformation("Buscando unidades para o tenant {TenantId}", tenantId);
        return await _repository.GetAllAsync(tenantId);
    }

    public async Task<IEnumerable<Unidade>> GetAllUnidadesAdminAsync()
    {
        _logger.LogInformation("Buscando todas as unidades (admin)");
        return await _repository.GetAllAdminAsync();
    }

    public async Task<Unidade> CreateUnidadeAsync(UnidadeDto unidadeDto, Guid userId, Guid tenantId)
    {
        if (unidadeDto == null)
        {
            throw new ArgumentNullException(nameof(unidadeDto));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId não pode ser vazio", nameof(userId));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId não pode ser vazio", nameof(tenantId));
        }

        // Validar DTO
        ValidateUnidadeDto(unidadeDto);

        _logger.LogInformation(
            "Criando unidade {Nome} para o usuário {UserId} e tenant {TenantId}",
            unidadeDto.Nome, userId, tenantId);

#pragma warning disable CS8601 // Possible null reference assignment.
        var novaUnidade = new Unidade
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Nome = unidadeDto.Nome?.Trim(),
            Descricao = unidadeDto.Descricao?.Trim(),
            Endereco = unidadeDto.Endereco?.Trim(),
            MetaMensal = unidadeDto.MetaMensal,
            DataInicio = EnsureUtc(unidadeDto.DataInicio),
            DataFim = EnsureNullableUtc(unidadeDto.DataFim),
            IsAtiva = true,
            UserId = userId

            // TODO: Adicionar os novos campos de customização
        };
#pragma warning restore CS8601 // Possible null reference assignment.

        try
        {
            await _repository.AddAsync(novaUnidade);

            var novoVinculo = new UsuarioOperacao
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UnidadeId = novaUnidade.Id,
                TenantId = tenantId,
                RoleInOperation = "Gerente"
            };

            await _repository.AddUsuarioOperacaoLinkAsync(novoVinculo);
            await _repository.SaveChangesAsync();

            _logger.LogInformation(
                "Unidade {UnidadeId} criada com sucesso",
                novaUnidade.Id);

            return novaUnidade;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao criar unidade {Nome} para o tenant {TenantId}",
                unidadeDto.Nome, tenantId);
            throw;
        }
    }

    public async Task<Unidade?> GetUnidadeByIdAsync(Guid id, Guid tenantId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id não pode ser vazio", nameof(id));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId não pode ser vazio", nameof(tenantId));
        }

        _logger.LogDebug("Buscando unidade {UnidadeId} para o tenant {TenantId}", id, tenantId);
        return await _repository.GetByIdAsync(id, tenantId);
    }

    public async Task<bool> UpdateUnidadeAsync(Guid id, UpdateUnidadeDto unidadeDto, Guid tenantId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id não pode ser vazio", nameof(id));
        }

        if (unidadeDto == null)
        {
            throw new ArgumentNullException(nameof(unidadeDto));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId não pode ser vazio", nameof(tenantId));
        }

        // Validar DTO
        ValidateUpdateUnidadeDto(unidadeDto);

        _logger.LogInformation("Atualizando unidade {UnidadeId} para o tenant {TenantId}", id, tenantId);

        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);

        if (unidadeExistente == null)
        {
            _logger.LogWarning("Unidade {UnidadeId} não encontrada para o tenant {TenantId}", id, tenantId);
            return false;
        }

        try
        {
            // Atualizar apenas os campos que foram fornecidos no DTO
            unidadeExistente.Nome = unidadeDto.Nome?.Trim() ?? unidadeExistente.Nome;
            unidadeExistente.Descricao = unidadeDto.Descricao?.Trim() ?? unidadeExistente.Descricao;
            unidadeExistente.Endereco = unidadeDto.Endereco?.Trim() ?? unidadeExistente.Endereco;
            unidadeExistente.MetaMensal = unidadeDto.MetaMensal;
            unidadeExistente.DataInicio = EnsureUtc(unidadeDto.DataInicio);
            unidadeExistente.DataFim = EnsureNullableUtc(unidadeDto.DataFim);

            // TODO: Mapear os campos de customização

            _repository.Update(unidadeExistente);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Unidade {UnidadeId} atualizada com sucesso", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao atualizar unidade {UnidadeId} para o tenant {TenantId}",
                id, tenantId);
            throw;
        }
    }

    public async Task<bool> DeactivateUnidadeAsync(Guid id, Guid tenantId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id não pode ser vazio", nameof(id));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId não pode ser vazio", nameof(tenantId));
        }

        _logger.LogInformation("Desativando unidade {UnidadeId} para o tenant {TenantId}", id, tenantId);

        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);

        if (unidadeExistente == null)
        {
            _logger.LogWarning("Unidade {UnidadeId} não encontrada para o tenant {TenantId}", id, tenantId);
            return false;
        }

        if (!unidadeExistente.IsAtiva)
        {
            _logger.LogWarning("Unidade {UnidadeId} já está desativada", id);
            return true; // Já está desativada, consideramos sucesso
        }

        try
        {
            unidadeExistente.IsAtiva = false;
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Unidade {UnidadeId} desativada com sucesso", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao desativar unidade {UnidadeId} para o tenant {TenantId}",
                id, tenantId);
            throw;
        }
    }

    public async Task<bool> DeleteUnidadeAsync(Guid id, Guid tenantId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id não pode ser vazio", nameof(id));
        }

        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("TenantId não pode ser vazio", nameof(tenantId));
        }

        _logger.LogInformation("Excluindo unidade {UnidadeId} para o tenant {TenantId}", id, tenantId);

        var unidadeExistente = await _repository.GetByIdAsync(id, tenantId);

        if (unidadeExistente == null)
        {
            _logger.LogWarning("Unidade {UnidadeId} não encontrada para o tenant {TenantId}", id, tenantId);
            return false;
        }

        try
        {
            // Verificar se há dependências antes de excluir
            // TODO: Adicionar validação de dependências se necessário

            _repository.Remove(unidadeExistente);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Unidade {UnidadeId} excluída com sucesso", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao excluir unidade {UnidadeId} para o tenant {TenantId}",
                id, tenantId);
            throw;
        }
    }

    public async Task<bool> UpdateProjecaoAsync(Guid id, decimal projecao)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id não pode ser vazio", nameof(id));
        }

        if (projecao < 0)
        {
            throw new ArgumentException("Projeção não pode ser negativa", nameof(projecao));
        }

        _logger.LogInformation("Atualizando projeção da unidade {UnidadeId} para {Projecao}", id, projecao);

        try
        {
            var sucesso = await _repository.UpdateProjecaoAsync(id, projecao);

            if (sucesso)
            {
                _logger.LogDebug("Projeção da unidade {UnidadeId} atualizada com sucesso", id);
            }
            else
            {
                _logger.LogWarning("Falha ao atualizar projeção da unidade {UnidadeId}", id);
            }

            return sucesso;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao atualizar projeção da unidade {UnidadeId}",
                id);
            throw;
        }
    }

    #region Métodos Auxiliares Privados

    private void ValidateUnidadeDto(UnidadeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
        {
            throw new ArgumentException("Nome da unidade é obrigatório", nameof(dto.Nome));
        }

        if (dto.MetaMensal < 0)
        {
            throw new ArgumentException("Meta mensal não pode ser negativa", nameof(dto.MetaMensal));
        }

        if (dto.DataInicio > DateTime.UtcNow.AddYears(1))
        {
            throw new ArgumentException("Data de início não pode ser mais de um ano no futuro", nameof(dto.DataInicio));
        }

        if (dto.DataFim.HasValue && dto.DataFim.Value < dto.DataInicio)
        {
            throw new ArgumentException("Data de fim não pode ser anterior à data de início", nameof(dto.DataFim));
        }
    }

    private void ValidateUpdateUnidadeDto(UpdateUnidadeDto dto)
    {
        if (!string.IsNullOrWhiteSpace(dto.Nome) && dto.Nome.Trim().Length < 2)
        {
            throw new ArgumentException("Nome da unidade deve ter pelo menos 2 caracteres", nameof(dto.Nome));
        }

        if (dto.MetaMensal < 0)
        {
            throw new ArgumentException("Meta mensal não pode ser negativa", nameof(dto.MetaMensal));
        }

        if (dto.DataFim.HasValue && dto.DataFim.Value < dto.DataInicio)
        {
            throw new ArgumentException("Data de fim não pode ser anterior à data de início", nameof(dto.DataFim));
        }
    }

    private DateTime EnsureUtc(DateTime dateTime)
    {
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    private DateTime? EnsureNullableUtc(DateTime? dateTime)
    {
        if (!dateTime.HasValue)
        {
            return null;
        }

        return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
    }

    #endregion
}