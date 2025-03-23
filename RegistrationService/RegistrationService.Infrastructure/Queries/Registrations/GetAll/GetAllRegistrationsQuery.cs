using RegistrationService.Core.Commons;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Infrastructure.Commons;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetAll;

public class GetAllRegistrationsQuery : BaseRequest<BaseResponse<List<RegistrationDetails>>>
{
    public Guid? GroupId { get; init; }
}