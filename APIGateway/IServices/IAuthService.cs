using APIGatewat.Models;

namespace APIGatewat.IServices;

public interface IAuthService
{
    Task<AuthorizationResult> Login(LoginCredentials credentials);
    Task<AuthorizationResult> Register(NewUser newUser);
}