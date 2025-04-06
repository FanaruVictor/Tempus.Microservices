using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetAllEmails;

public class GetAllEmailsQuery : BaseRequest<BaseResponse<List<UserEmail>>>
{
}