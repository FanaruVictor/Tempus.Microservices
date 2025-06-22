using Microsoft.AspNetCore.Http;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommand : BaseRequest<BaseResponse<GroupOverview>>
{
	public string Name { get; set; }
	public string? Members { get; set; }
	public IFormFile? Image { get; set; }
}