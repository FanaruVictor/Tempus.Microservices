using MediatR;
using RegistrationService.Core.Commons;
using RegistrationService.Core.Entities;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Data.Context;
using RegistrationService.Infrastructure.Commons;
using System.Data.Entity;

namespace RegistrationService.Infrastructure.Queries.Registrations.LastUpdated;

public class
    GetLastRegistrationUpdatedQueryHandler : IRequestHandler<GetLastUpdatedRegsitrationQuery,
        BaseResponse<BaseRegistration>>
{
    private readonly RegistrationServiceDbContext _context;

    public GetLastRegistrationUpdatedQueryHandler(RegistrationServiceDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<BaseRegistration>> Handle(GetLastUpdatedRegsitrationQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var registration = await _context.Registrations
                .AsNoTracking()
                .OrderByDescending(x => x.LastUpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (registration == null)
            {
                return BaseResponse<BaseRegistration>.NotFound("Registration not found!");
            }

            var response =
                BaseResponse<BaseRegistration>.Ok(GenericMapper<Registration, BaseRegistration>.Map(registration));
            return response;
        }
        catch (Exception exception)
        {
            var response = BaseResponse<BaseRegistration>.BadRequest(new List<string> { exception.Message });
            return response;
        }
    }
}