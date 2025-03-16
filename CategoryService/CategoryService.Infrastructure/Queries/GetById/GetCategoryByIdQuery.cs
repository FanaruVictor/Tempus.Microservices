using CategoryService.Core.Models.Category;
using CategoryService.Infrastructure.Commons;

namespace CategoryService.Infrastructure.Queries.Categories.GetById;

public class GetCategoryByIdQuery : BaseRequest<BaseResponse<BaseCategory>>
{
    public Guid Id { get; init; }
}