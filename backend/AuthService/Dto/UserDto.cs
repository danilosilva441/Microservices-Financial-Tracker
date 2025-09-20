namespace AuthService.DTO; // <-- ESTA LINHA É A MAIS IMPORTANTE

public class UserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}