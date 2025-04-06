using Tempus.Shared.Commons;
using Tempus.Shared.Models.Registration;

namespace RegistrationService.Infrastructure.Commands.Registrations.Update;

public class UpdateRegistrationCommand : BaseRequest<BaseResponse<RegistrationDetails>>
{
    public Guid Id { get; init; }
    public string? Description { get; init; }
    public string? Content { get; init; }
    public Guid? GroupId { get; set; }
}