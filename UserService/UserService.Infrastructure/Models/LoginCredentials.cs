namespace UserService.Infrastructure.Models;

public sealed class LoginCredentials
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}