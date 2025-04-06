using Tempus.Shared.Commons;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Queries.Registrations.LastUpdated;

public class GetLastUpdatedRegsitrationQuery : BaseRequest<BaseResponse<RegistrationDetails>>
{
    public Guid? GroupId { get; set; }
}