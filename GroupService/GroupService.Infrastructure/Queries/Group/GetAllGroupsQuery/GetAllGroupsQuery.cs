using GroupService.Core.Commons;
using GroupService.Infrastructure.Commons;
using GroupService.Infrastructure.Models;

namespace GroupService.Infrastructure.Queries.Group.GetAllGroupsQuery;

public class GetAllGroupsQuery : BaseRequest<BaseResponse<List<GroupOverview>>>
{

}