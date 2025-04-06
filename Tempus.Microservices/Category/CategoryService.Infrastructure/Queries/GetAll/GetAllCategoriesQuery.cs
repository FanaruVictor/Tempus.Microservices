using Tempus.Shared.Commons;
using Tempus.Shared.Models.Category;

namespace CategoryService.Infrastructure.Queries.Categories.GetAll;

public class GetAllCategoriesQuery : BaseRequest<BaseResponse<List<BaseCategory>>>
{
    public Guid? GroupId { get; set; }
}