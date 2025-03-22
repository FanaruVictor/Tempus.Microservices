using MediatR;
using Microsoft.EntityFrameworkCore;
using RegistrationService.Core.Commons;
using RegistrationService.Core.Entities;
using RegistrationService.Data.Context;

namespace RegistrationService.Infrastructure.Commands.Registrations.Delete;

public class DeleteRegistrationCommandHandler : IRequestHandler<DeleteRegistrationCommand, BaseResponse<Guid>>
{
    private readonly RegistrationServiceDbContext _context;

    public DeleteRegistrationCommandHandler(RegistrationServiceDbContext context)
    {
        _context = context;
    }

    public async Task<BaseResponse<Guid>> Handle(DeleteRegistrationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var registration = await _context.Registrations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (registration == null)
            {
                return BaseResponse<Guid>.NotFound("Registration not found!");
            }

            var validator = ValidateRequest(request, registration);

            if (validator.StatusCode != StatusCodes.Ok)
            {
                return validator;
            }

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync(cancellationToken);
            /*
            should send event to all users from the group the registration was in
            if (request.GroupId.HasValue)
            {
                var usersId = await _groupUserRepository.GetAllUsersFromGroup(request.GroupId.Value);
                SendEvent(usersId, registration.Id, request.GroupId.Value);
            }
            */


            return BaseResponse<Guid>.Ok(registration.Id);
        }
        catch (Exception exception)
        {
            return BaseResponse<Guid>.BadRequest(new List<string> { exception.Message });
        }
    }

    private BaseResponse<Guid> ValidateRequest(DeleteRegistrationCommand request,
        Registration registration)
    {
        if (request.GroupId.HasValue)
        {
            return ValidateForGroup(request, registration);
        }

        return ValidateForUser(request, registration);
    }

    private BaseResponse<Guid> ValidateForUser(DeleteRegistrationCommand request,
        Registration registration)
    {

        //get userId for registration
        Guid userId = Guid.Empty;

        if (userId == null)
        {
            return BaseResponse<Guid>.BadRequest(new List<string> { "Internal server error" });
        }

        if (userId != request.UserId)
        {
            return BaseResponse<Guid>.Forbbiden();
        }

        return BaseResponse<Guid>.Ok();
    }

    private BaseResponse<Guid> ValidateForGroup(DeleteRegistrationCommand request,
        Registration registration)
    {

        // get the groupId on which this registration was created
        Guid groupId = Guid.Empty;

        if (groupId == null)
        {
            return BaseResponse<Guid>.NotFound("Group not found!");
        }

        if (groupId != request.GroupId)
        {
            return BaseResponse<Guid>.Forbbiden();
        }

        return BaseResponse<Guid>.Ok();
    }

    /* private async Task SendEvent(List<Guid> usersId, Guid registrationId, Guid groupId)
     {
         foreach (var userId in usersId)
         {
             await _clientEventSender.SendRegistrationDeleted(registrationId, groupId, userId.ToString());
         }
     }*/
}