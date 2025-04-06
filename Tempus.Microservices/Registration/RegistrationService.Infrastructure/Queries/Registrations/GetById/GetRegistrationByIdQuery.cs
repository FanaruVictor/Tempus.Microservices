using Tempus.Shared.Commons;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetById;

public class GetRegistrationByIdQuery : BaseRequest<BaseResponse<RegistrationDetails>>
{
    public Guid Id { get; init; }
    public Guid? GroupId { get; set; }
}