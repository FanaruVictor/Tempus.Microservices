using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetAll;

public class GetAllUsersQuery : BaseRequest<BaseResponse<List<UserDetails>>> { }