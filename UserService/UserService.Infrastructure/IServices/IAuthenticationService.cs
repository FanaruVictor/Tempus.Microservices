using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.IServices;

public interface IAuthenticationService
{
    Task<BaseResponse<LoginResult>> Login(LoginCredentials credentials, CancellationToken cancellationToken);
}