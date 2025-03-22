using MediatR;
using GroupService.Core.Commons;
using GroupService.Core.Models.Group;
using GroupService.Infrastructure.Commons;

namespace GroupService.Infrastructure.Queries.Group.GetGroupByIdQuery;

public class GetGroupByIdQuery : BaseRequest<BaseResponse<GroupDetails>>
{
    public Guid Id { get; set; }
}