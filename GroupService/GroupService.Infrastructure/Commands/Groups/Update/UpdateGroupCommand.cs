using Microsoft.AspNetCore.Http;
using GroupService.Core.Commons;
using GroupService.Core.Models.Group;
using GroupService.Infrastructure.Commons;

namespace GroupService.Infrastructure.Commands.Groups.Update;

public class UpdateGroupCommand : BaseRequest<BaseResponse<GroupOverview>>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Members { get; set; }
    public IFormFile? Image { get; set; }
    public bool IsCurrentImageChanged { get; set; }
}