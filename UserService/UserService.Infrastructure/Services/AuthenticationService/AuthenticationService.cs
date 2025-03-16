using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Core.Entities;
using UserService.Data.Context;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.IServices;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Services.AuthenticationService;

public class AuthenticationService(UserServiceDbContext context, IConfiguration configuration) : IAuthenticationService
{
    private readonly UserServiceDbContext _context = context;
    private readonly IConfiguration _configuration = configuration;

    public async Task<BaseResponse<LoginResult>> Login(LoginCredentials credentials,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await IsEmailAlreadyRegistered(credentials.Email) && !await IsExternalIdAlreadyRegistered(credentials.ExternalId))
            {
                await Register(new RegistrationData
                {
                    Email = credentials.Email,
                    UserName = credentials.UserName,
                    ExternalId = credentials.ExternalId,
                    PhoneNumber = credentials.PhoneNumber,
                    PhotoURL = credentials.PhotoURL
                }, cancellationToken);
            }

            var user = await Login(credentials.Email, credentials.ExternalId);

            CreateToken(user, out var tokenHandler, out var token);

            var result = new LoginResult
            {
                User = GenericMapper<User, UserDetails>.Map(user),
                AuthorizationToken = tokenHandler.WriteToken(token)
            };

            var profilePhoto = await _context.UserPhotos.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

            if (profilePhoto != null)
            {
                result.User.Photo = GenericMapper<UserPhoto, PhotoDetails>.Map(profilePhoto);
            }

            var response = BaseResponse<LoginResult>.Ok(result);

            return response;
        }
        catch (TaskCanceledException canceledException)
        {
            return BaseResponse<LoginResult>.BadRequest([canceledException.Message]);
        }
        catch (Exception exception)
        {
            var response = BaseResponse<LoginResult>.BadRequest([exception.Message]);
            return response;
        }
    }

    public async Task Register(User user)
    {
        await _context.Users.AddAsync(user);
    }


    private async Task<User> Login(string email, string externalId)
    {
        email = email.ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(x =>
            x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase) && x.ExternalId == externalId);

        return user;
    }

    private async Task<bool> IsEmailAlreadyRegistered(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase));
    }

    private async Task<bool> IsExternalIdAlreadyRegistered(string externalId)
    {
        return await _context.Users.AnyAsync(x => x.ExternalId == externalId);
    }
    private async Task<BaseResponse<UserDetails>> Register(RegistrationData userInfo,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await IsEmailAlreadyRegistered(userInfo.Email.ToLower()))
            {
                throw new Exception("Email already registered");
            }

            var entity = await RegisterUser(userInfo);

            if (userInfo.PhotoURL != "")
            {
                await _context.UserPhotos.AddAsync(new UserPhoto
                {
                    Id = Guid.NewGuid(),
                    UserId = entity.Id,
                    PublicId = "",
                    Url = userInfo.PhotoURL,
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            var baseUser = GenericMapper<User, UserDetails>.Map(entity);
            var result = BaseResponse<UserDetails>.Ok(baseUser);

            return result;
        }
        catch (TaskCanceledException canceledException)
        {
            return BaseResponse<UserDetails>.BadRequest([canceledException.Message]);
        }
        catch (Exception exception)
        {
            return BaseResponse<UserDetails>.BadRequest([exception.Message]);
        }
    }

    private async Task<User> RegisterUser(RegistrationData userInfo)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = userInfo.UserName,
            Email = userInfo.Email,
            PhoneNumber = userInfo.PhoneNumber,
            ExternalId = userInfo.ExternalId
        };

        await Register(user);

        return user;
    }

    private void CreateToken(User user, out JwtSecurityTokenHandler tokenHandler, out SecurityToken token)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

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