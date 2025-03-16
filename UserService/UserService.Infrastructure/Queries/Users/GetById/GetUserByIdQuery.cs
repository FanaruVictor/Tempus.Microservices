using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetById;

public class GetUserByIdQuery : BaseRequest<BaseResponse<UserDetails>>
{
    public Guid Id { get; set; }
}