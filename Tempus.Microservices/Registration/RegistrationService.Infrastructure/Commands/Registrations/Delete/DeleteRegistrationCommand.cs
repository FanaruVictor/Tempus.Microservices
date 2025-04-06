using Tempus.Shared.Commons;

namespace RegistrationService.Infrastructure.Commands.Registrations.Delete;

public class DeleteRegistrationCommand : BaseRequest<BaseResponse<Guid>>
{
    public Guid Id { get; init; }
    public Guid? GroupId { get; init; }
}