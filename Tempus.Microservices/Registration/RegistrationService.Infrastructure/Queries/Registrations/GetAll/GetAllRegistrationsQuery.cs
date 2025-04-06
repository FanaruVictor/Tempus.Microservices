using Tempus.Shared.Commons;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetAll;

public class GetAllRegistrationsQuery : BaseRequest<BaseResponse<List<RegistrationDetails>>>
{
    public Guid? GroupId { get; init; }
}