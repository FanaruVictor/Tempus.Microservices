using GroupService.Core.Commons;
using GroupService.Infrastructure.Commons;
using GroupService.Infrastructure.Models;

namespace GroupService.Infrastructure.Queries.Groups.GetGroupByIdQuery;

public class GetGroupByIdQuery : BaseRequest<BaseResponse<GroupDetails>>
{
    public Guid Id { get; set; }
}