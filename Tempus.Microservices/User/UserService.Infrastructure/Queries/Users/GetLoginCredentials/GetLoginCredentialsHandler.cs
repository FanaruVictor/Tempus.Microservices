using Tempus.Shared.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetLoginCredentials
{
    public class GetLoginCredentialsHandler : BaseRequest<BaseResponse<LoginCredentials>>
    {
        public string Email { get; set; }
    }
}
