using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Commands.Users.ChangeTheme;

public class ChangeThemeCommand : BaseRequest<BaseResponse<UserDetails>>
{
    public bool IsDarkTheme { get; set; }
}