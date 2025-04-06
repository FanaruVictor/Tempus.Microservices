using Tempus.Shared.Commons;
using Tempus.Shared.Models.User;
using UserService.Infrastructure.Models;

namespace UserService.Infrastructure.Queries.Users.GetUserEmails
{
    public class GetUserEmailsQuery : BaseRequest<BaseResponse<List<UserEmail>>>
    {
        public List<Guid> UserIds { get; set; }
    }
}
