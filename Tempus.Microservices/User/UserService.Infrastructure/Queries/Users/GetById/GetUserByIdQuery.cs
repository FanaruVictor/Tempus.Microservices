using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetById;

public class GetUserByIdQuery : BaseRequest<BaseResponse<UserDetails>>
{
    public Guid Id { get; set; }
}