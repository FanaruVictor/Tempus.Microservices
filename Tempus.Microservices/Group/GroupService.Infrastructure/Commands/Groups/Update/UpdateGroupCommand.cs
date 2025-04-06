using Microsoft.AspNetCore.Http;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Commands.Groups.Update;

public class UpdateGroupCommand : BaseRequest<BaseResponse<GroupOverview>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Members { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsCurrentImageChanged { get; set; }
}