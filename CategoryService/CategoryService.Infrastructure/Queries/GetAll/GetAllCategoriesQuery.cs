using CategoryService.Core.Models.Category;
using CategoryService.Infrastructure.Commons;

namespace CategoryService.Infrastructure.Queries.Categories.GetAll;

public class GetAllCategoriesQuery : BaseRequest<BaseResponse<List<BaseCategory>>>
{
    public Guid? GroupId { get; set; }
}