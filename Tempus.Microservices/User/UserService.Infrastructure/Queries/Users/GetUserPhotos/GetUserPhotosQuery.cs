using Tempus.Shared.Commons;

namespace UserService.Infrastructure.Queries.Users.GetUserPhotos
{
    public class GetUserPhotosQuery : BaseRequest<BaseResponse<List<string>>>
    {
        public List<Guid> UserIds { get; set; }
    }
}
