using Tempus.Shared.Commons;

namespace RegistrationService.Infrastructure.Commands.Registrations.DeleteAllForCategory
{
    public class DeleteAllRegistrationsForCategoryCommand : BaseRequest<BaseResponse<bool>>
    {
        public string CategoryId { get; set; }
    }
}
