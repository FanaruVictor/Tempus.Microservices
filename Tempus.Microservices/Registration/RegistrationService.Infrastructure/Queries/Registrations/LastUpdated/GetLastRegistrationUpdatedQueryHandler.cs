using MediatR;
using Microsoft.EntityFrameworkCore;
using RegistrationService.Core.Entities;
using RegistrationService.Data.Context;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Registration;

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
                .Where(x => x.OwnerId == (request.GroupId.HasValue && request.GroupId.Value != Guid.Empty
                    ? request.GroupId.Value
                    : request.UserId))
                .OrderByDescending(x => x.LastUpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if(registration == null)
            {
                return BaseResponse<RegistrationDetails>.Ok(null);
            }

            var response =
                BaseResponse<RegistrationDetails>.Ok(
                    GenericMapper<Registration, RegistrationDetails>.Map(registration));
            return response;
        }
        catch(Exception exception)
        {
            var response = BaseResponse<RegistrationDetails>.BadRequest(new List<string> {exception.Message});
            return response;
        }
    }
}