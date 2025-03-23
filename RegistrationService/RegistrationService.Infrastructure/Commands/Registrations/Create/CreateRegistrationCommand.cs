using RegistrationService.Core.Commons;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Infrastructure.Commons;

namespace RegistrationService.Infrastructure.Commands.Registrations.Create;

public class CreateRegistrationCommand : BaseRequest<BaseResponse<RegistrationDetails>>
{
    public string? Description { get; init; }
    public string? Content { get; init; }
    public Guid CategoryId { get; init; }
    public Guid? GroupId { get; set; }
}