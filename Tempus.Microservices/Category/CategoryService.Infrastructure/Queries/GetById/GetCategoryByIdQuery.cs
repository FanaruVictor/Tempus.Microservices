using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;

namespace CategoryService.Infrastructure.Queries.Categories.GetById;

public class GetCategoryByIdQuery : BaseRequest<BaseResponse<BaseCategory>>
{
    public Guid Id { get; init; }
    public Guid? GroupId { get; set; }
}