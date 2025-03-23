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
        BaseResponse<RegistrationDetails>>
{
    private readonly RegistrationServiceDbContext _context;

    public GetLastRegistrationUpdatedQueryHandler(RegistrationServiceDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<RegistrationDetails>> Handle(GetLastUpdatedRegsitrationQuery request,
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
                return BaseResponse<RegistrationDetails>.NotFound("Registration not found!");
            }

            var response =
                BaseResponse<RegistrationDetails>.Ok(GenericMapper<Registration, RegistrationDetails>.Map(registration));
            return response;
        }
        catch (Exception exception)
        {
            var response = BaseResponse<RegistrationDetails>.BadRequest(new List<string> { exception.Message });
            return response;
        }
    }
}