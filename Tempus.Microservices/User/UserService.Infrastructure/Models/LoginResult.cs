using Tempus.Shared.Models.User;

namespace UserService.Infrastructure.Models;

public class LoginResult
{
    public UserDetails User { get; set; }

    public string AuthorizationToken { get; set; }
}