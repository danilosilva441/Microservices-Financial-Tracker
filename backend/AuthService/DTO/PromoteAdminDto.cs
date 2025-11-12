// Caminho: backend/AuthService/DTO/PromoteAdminDto.cs
namespace AuthService.DTO
{
    // DTO usado pelo AdminController e pelo script create-system-user.sh
    public class PromoteAdminDto
    {
        public string Email { get; set; } = string.Empty;
    }
}