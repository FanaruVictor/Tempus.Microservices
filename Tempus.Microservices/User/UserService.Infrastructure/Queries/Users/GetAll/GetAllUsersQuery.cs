using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;

namespace UserService.Infrastructure.Queries.Users.GetAll;

public class GetAllUsersQuery : BaseRequest<BaseResponse<List<UserDetails>>> { }