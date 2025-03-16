using UserService.Infrastructure.Commons;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetEmails;

public class GetEmailsQuery : BaseRequest<BaseResponse<List<UserEmail>>>
{
}