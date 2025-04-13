using MediatR;
using Microsoft.EntityFrameworkCore;
using RegistrationService.Data.Context;
using Tempus.Shared.Commons;

namespace RegistrationService.Infrastructure.Commands.Registrations.DeleteAllForCategory
{
    public class DeleteAllRegistrationsForCategoryCommandHandler(RegistrationServiceDbContext context) : IRequestHandler<DeleteAllRegistrationsForCategoryCommand, BaseResponse<bool>>
    {
        private readonly RegistrationServiceDbContext context = context;

        public async Task<BaseResponse<bool>> Handle(DeleteAllRegistrationsForCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guid categoryId = Guid.Empty;

                if (!Guid.TryParse(request.CategoryId, out categoryId))
                {
                    return BaseResponse<bool>.NotFound("Category not found");
                }

                var registrations = await context.Registrations
                    .AsNoTracking()
                    .Where(x => x.CategoryId == categoryId)
                    .ToListAsync(cancellationToken);

                /*
                should send event to all users from the group the registration was in
                if (request.GroupId.HasValue)
                {
                    var usersId = await _groupUserRepository.GetAllUsersFromGroup(request.GroupId.Value);
                    SendEvent(usersId, registration.Id, request.GroupId.Value);
                }
                */

                context.RemoveRange(registrations);

                await context.SaveChangesAsync(cancellationToken);

                return BaseResponse<bool>.Ok(true);
            }
            catch (Exception exception)
            {
                return BaseResponse<bool>.BadRequest(new List<string> { exception.Message });
            }
        }

        /* private async Task SendEvent(List<Guid> usersId, Guid registrationId, Guid groupId)
         {
             foreach (var userId in usersId)
             {
                 await _clientEventSender.SendRegistrationDeleted(registrationId, groupId, userId.ToString());
             }
         }*/
    }
}
