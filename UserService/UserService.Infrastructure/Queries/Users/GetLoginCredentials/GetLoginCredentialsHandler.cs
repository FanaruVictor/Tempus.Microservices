using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetUserByEmail
{
    public class GetLoginCredentialsHandler : BaseRequest<BaseResponse<LoginCredentials>>
    {
        public string Email { get; set; }
    }
}
