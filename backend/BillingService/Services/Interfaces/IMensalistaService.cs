using BillingService.DTO;
using BillingService.DTOs;
using BillingService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BillingService.Services.Interfaces
{
    public interface IMensalistaService
    {
        // Método corrigido - agora retorna MensalistaDto
        Task<IEnumerable<MensalistaDto>> GetAllMensalistasAsync(
            Guid operacaoId, 
            Guid userId);  // Mudei de tenantId para userId para consistência
        
        Task<(MensalistaDto? mensalista, string? errorMessage)> CreateMensalistaAsync(
            Guid operacaoId, 
            CreateMensalistaDto mensalistaDto, 
            Guid userId);
        
        Task<(bool success, string? errorMessage)> UpdateMensalistaAsync(
            Guid operacaoId, 
            Guid mensalistaId, 
            UpdateMensalistaDto mensalistaDto, 
            Guid userId);
        
        Task<(bool success, string? errorMessage)> DeactivateMensalistaAsync(
            Guid operacaoId, 
            Guid mensalistaId, 
            Guid userId);
        
        // Método corrigido - retorna MensalistaDto
        Task<MensalistaDto?> GetMensalistaByIdAsync(
            Guid operacaoId, 
            Guid mensalistaId, 
            Guid userId);
    }
}