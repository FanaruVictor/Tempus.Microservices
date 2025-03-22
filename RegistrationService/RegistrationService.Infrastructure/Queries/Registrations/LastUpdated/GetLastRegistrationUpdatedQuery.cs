using RegistrationService.Core.Commons;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Infrastructure.Commons;

namespace RegistrationService.Infrastructure.Queries.Registrations.LastUpdated;

public class GetLastUpdatedRegsitrationQuery : BaseRequest<BaseResponse<BaseRegistration>> { }