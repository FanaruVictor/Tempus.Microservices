using Tempus.Shared.Models.User;

namespace APIGatewat.Models;

public class AuthorizationResult
{
	public string AuthorizationToken { get; set; }
	public UserDetails UserDetails { get; set; }
}