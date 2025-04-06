using Microsoft.AspNetCore.Http;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Commands.Users.Update;

public class UpdateUserCommand : BaseRequest<BaseResponse<UserDetails>>
{
    public string UserName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsPhotoChanged { get; set; }
    public IFormFile? NewPhoto { get; set; }
}