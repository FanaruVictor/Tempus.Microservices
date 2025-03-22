using GroupService.Core.Models;

namespace GroupService.Infrastructure.Queries.Category.GetAll
{
    public class GetAllCategoriesQuery : BaseRequest<BaseResponse<List<BaseCategory>>>

    {
        public Guid GroupId { get; set; }
    }
}
