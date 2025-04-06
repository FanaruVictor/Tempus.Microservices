using Tempus.Shared.Commons;

namespace GroupService.Infrastructure.Commands.Groups.Delete;

public class DeleteGroupCommand : BaseRequest<BaseResponse<Guid>>
{
    public Guid Id { get; set; }
}