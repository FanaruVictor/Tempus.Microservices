using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetAllEmails;

public class GetAllEmailsQuery : BaseRequest<BaseResponse<List<UserEmail>>>
{
}