using Tempus.Shared.Commons;

namespace CategoryService.Infrastructure.Commands.UserCategory.Delete;

public class DeleteCategoryCommand : BaseRequest<BaseResponse<Guid>>
{
    public Guid Id { get; init; }
    public Guid? GroupId { get; set; }
}