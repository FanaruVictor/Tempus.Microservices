using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;

namespace CategoryService.Infrastructure.Commands.UserCategory.Create;

public class CreateCategoryCommand : BaseRequest<BaseResponse<BaseCategory>>
{
    public string Name { get; init; }
    public string? Color { get; init; }
    public Guid? GroupId { get; set; }
}