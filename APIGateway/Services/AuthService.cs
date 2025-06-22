using APIGatewat.Models;
using APIGateway.Models;
using APIGateway.Models.User;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tempus.Shared.Models.User;
using BCryptNet = BCrypt.Net;

namespace APIGatewat.IServices;

public class AuthService : IAuthService
{
	private readonly IConfiguration configuration;
	private readonly string userServiceBaseUrl;
	private readonly HttpClient httpClient;

	public AuthService (IConfiguration configuration, IHttpClientFactory httpClientFactory)
	{
		this.configuration = configuration;

		httpClient = httpClientFactory.CreateClient("userservice-api");
	}

	public async Task<AuthorizationResult> Register (NewUser newUser)
	{

		newUser.Password = BCryptNet.BCrypt.HashPassword(newUser.Password);

		string json = JsonConvert.SerializeObject(newUser);

		HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

		var responseObject = await httpClient.PostAsync("/api/users", content);

		responseObject.EnsureSuccessStatusCode();

		string responseBody = await responseObject.Content.ReadAsStringAsync();

		var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseBody);

		CreateToken(response.Resource, out var tokenHandler, out var token);

		var result = new AuthorizationResult
		{
			AuthorizationToken = tokenHandler.WriteToken(token),
			UserDetails = response.Resource
		};

		return result;
	}

	public async Task<AuthorizationResult?> Login (LoginCredentials credentials)
	{
		var responseObject = await httpClient.GetAsync($"/api/users/loginCredentials/{credentials.Email}");

		responseObject.EnsureSuccessStatusCode();

		string responseBody = await responseObject.Content.ReadAsStringAsync();

		var response = JsonConvert.DeserializeObject<HttpResponse<LoginResult>>(responseBody);

		if (response?.Resource == null || !BCryptNet.BCrypt.Verify(credentials.Password, response.Resource.Password))
		{
			return null;
		}

		var user = new UserDetails
		{
			Id = response.Resource.Id,
			UserName = response.Resource.UserName,
		};

		CreateToken(user, out var tokenHandler, out var token);

		var result = new AuthorizationResult
		{
			AuthorizationToken = tokenHandler.WriteToken(token),
			UserDetails = user
		};

		return result;
	}

	private void CreateToken (UserDetails user, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.UserName)
		};

		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(this.configuration.GetSection("AppSettings:Token").Value!));

		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.Now.AddDays(1),
			SigningCredentials = credentials
		};

		tokenHandler = new JwtSecurityTokenHandler();

		token = tokenHandler.CreateToken(tokenDescriptor);
	}
}