using APIGatewat.Models;
using APIGateway.Models;
using APIGateway.Models.User;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCryptNet = BCrypt.Net;

namespace APIGatewat.IServices;

public class AuthService : IAuthService
{
    private readonly IConfiguration configuration;
    private readonly string userServiceBaseUrl;

    public AuthService(IConfiguration configuration)
    {
        this.configuration = configuration;

        this.userServiceBaseUrl = configuration["userServiceBaseURL"];
    }

    public async Task<AuthorizationResult> Register(NewUser newUser)
    {

        using var httpClient = new System.Net.Http.HttpClient();

        newUser.Password = BCryptNet.BCrypt.HashPassword(newUser.Password);

        string json = JsonConvert.SerializeObject(newUser);

        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

        var responseObject = await httpClient.PostAsync(new Uri(this.userServiceBaseUrl), content);

        responseObject.EnsureSuccessStatusCode();

        string responseBody = await responseObject.Content.ReadAsStringAsync();

        var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseBody);

        CreateToken(response.Resource, out var tokenHandler, out var token);

        var result = new AuthorizationResult
        {
            AuthorizationToken = tokenHandler.WriteToken(token)
        };

        return result;
    }

    public async Task<AuthorizationResult> Login(LoginCredentials credentials)
    {
        using var httpClient = new System.Net.Http.HttpClient();

        var responseObject = await httpClient.GetAsync(new Uri($"{this.userServiceBaseUrl}/loginCredentials/{credentials.Email}"));

        responseObject.EnsureSuccessStatusCode();

        string responseBody = await responseObject.Content.ReadAsStringAsync();

        var response = JsonConvert.DeserializeObject<HttpResponse<LoginResult>>(responseBody);

        if (!BCryptNet.BCrypt.Verify(credentials.Password, response.Resource.Password))
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
            AuthorizationToken = tokenHandler.WriteToken(token)
        };

        return result;
    }

    private void CreateToken(UserDetails user, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
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