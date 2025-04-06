using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Queries.Groups.GetAllGroupsQuery;

public class GetAllGroupsQuery : BaseRequest<BaseResponse<List<GroupOverview>>> { }