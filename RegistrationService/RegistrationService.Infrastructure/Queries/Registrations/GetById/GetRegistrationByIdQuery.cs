using RegistrationService.Core.Commons;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Infrastructure.Commons;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetById;

public class GetRegistrationByIdQuery : BaseRequest<BaseResponse<RegistrationDetails>>
{
    public Guid Id { get; init; }
    public Guid? GroupId { get; set; }
}