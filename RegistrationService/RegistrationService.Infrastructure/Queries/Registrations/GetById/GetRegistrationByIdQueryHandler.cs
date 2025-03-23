using MediatR;
using RegistrationService.Core.Commons;
using RegistrationService.Core.Entities;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Data.Context;
using RegistrationService.Infrastructure.Commons;
using System.Data.Entity;

namespace RegistrationService.Infrastructure.Queries.Registrations.GetById;

public class
    GetRegistrationByIdQueryHandler : IRequestHandler<GetRegistrationByIdQuery, BaseResponse<RegistrationDetails>>
{
    private readonly RegistrationServiceDbContext _context;

    public GetRegistrationByIdQueryHandler(RegistrationServiceDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<RegistrationDetails>> Handle(GetRegistrationByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var registration = await _context.Registrations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (registration == null)
            {
                return BaseResponse<RegistrationDetails>.NotFound("Registration not found!");
            }

            if ((request.GroupId.HasValue && registration.OwnerId != request.GroupId.Value) || registration.OwnerId != request.UserId)
            {
                return BaseResponse<RegistrationDetails>.Forbbiden();
            }

            var response =
                BaseResponse<RegistrationDetails>.Ok(
                    GenericMapper<Registration, RegistrationDetails>.Map(registration));
            return response;
        }
        catch (Exception exception)
        {
            var response = BaseResponse<RegistrationDetails>.BadRequest(new List<string> { exception.Message });
            return response;
        }
    }
}