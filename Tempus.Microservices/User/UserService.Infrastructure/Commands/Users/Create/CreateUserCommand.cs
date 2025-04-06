using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Commands.Users.Create
{
    public class CreateUserCommand : BaseRequest<BaseResponse<UserDetails>>
    {
        public string UserName { get; init; }
        public string Email { get; init; }
        public string Password { get; set; }
        public string? PhoneNumber { get; init; }
    }
}
