using Tempus.Shared.Commons;

namespace GroupService.Infrastructure.Commands.Groups.DeleteAllGroupsForUser
{
    public class DeleteAllGroupsForUserCommand : BaseRequest<BaseResponse<bool>>
    {
        public string UserId { get; set; }
    }
}
