using APIGatewat.Models;
using APIGateway.Models.User;

namespace APIGatewat.IServices;

public interface IAuthService
{
    Task<AuthorizationResult> Login(LoginCredentials credentials);
    Task<AuthorizationResult> Register(NewUser newUser);
}