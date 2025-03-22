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

            var validator = ValidateRequest(request, registration);

            if (validator.StatusCode != StatusCodes.Ok)
            {
                return validator;
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

    private BaseResponse<RegistrationDetails> ValidateRequest(GetRegistrationByIdQuery request,
        Registration registration)
    {
        if (request.GroupId.HasValue)
        {
            return ValidateForGroup(request, registration);
        }

        return ValidateForUser(request, registration);
    }

    private BaseResponse<RegistrationDetails> ValidateForUser(GetRegistrationByIdQuery request,
        Registration registration)
    {
        //get userId for registration
        Guid userId = Guid.Empty;

        if (userId == null)
        {
            return BaseResponse<RegistrationDetails>.BadRequest(new List<string> { "Internal server error" });
        }

        if (userId != request.UserId)
        {
            return BaseResponse<RegistrationDetails>.Forbbiden();
        }

        return BaseResponse<RegistrationDetails>.Ok();
    }

    private BaseResponse<RegistrationDetails> ValidateForGroup(GetRegistrationByIdQuery request,
        Registration registration)
    {
        // get the groupId on which this registration was created
        Guid groupId = Guid.Empty;

        if (groupId == null)
        {
            return BaseResponse<RegistrationDetails>.NotFound("Group not found!");
        }

        return BaseResponse<RegistrationDetails>.Ok();
    }
}