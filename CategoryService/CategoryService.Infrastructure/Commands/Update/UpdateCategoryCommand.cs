using CategoryService.Core.Models.Category;
using CategoryService.Infrastructure.Commons;

namespace CategoryService.Infrastructure.Commands.UserCategory.Update;

public class UpdateCategoryCommand : BaseRequest<BaseResponse<BaseCategory>>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? Color { get; init; }
    public Guid? GroupId { get; set; }

}