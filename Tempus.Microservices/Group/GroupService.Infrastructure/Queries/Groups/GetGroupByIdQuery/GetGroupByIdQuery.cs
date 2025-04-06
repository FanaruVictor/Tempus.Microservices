using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Queries.Groups.GetGroupByIdQuery;

public class GetGroupByIdQuery : BaseRequest<BaseResponse<GroupDetails>>
{
    public Guid Id { get; set; }
}