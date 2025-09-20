namespace AuthService.Dto;

// DTO (Data Transfer Object) para receber os dados de registro/login
public class UserDto
{
    // A palavra-chave 'required' garante que esses campos devem ser enviados na requisição
    public required string Email { get; set; }
    public required string Password { get; set; }
}