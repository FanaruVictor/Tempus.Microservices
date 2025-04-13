using Tempus.Shared.Commons;

namespace CategoryService.Infrastructure.Commands.Registrations.DeleteAllForOwner
{
    public class DeleteAllCategoriesForOwnerCommand : BaseRequest<BaseResponse<bool>>
    {
        public string OwnerId { get; set; }
    }
}
