using GroupService.Core.Commons;
using GroupService.Infrastructure.Commons;

namespace GroupService.Infrastructure.Commands.Groups.Delete;

public class DeleteGroupCommand : BaseRequest<BaseResponse<Guid>>
{
    public Guid Id { get; set; }
}