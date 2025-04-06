using Microsoft.AspNetCore.Http;
using Tempus.Shared.Commons;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommand : BaseRequest<BaseResponse<bool>>
{
    public string Name { get; set; }
    public string? Members { get; set; }
    public IFormFile? Image { get; set; }
}