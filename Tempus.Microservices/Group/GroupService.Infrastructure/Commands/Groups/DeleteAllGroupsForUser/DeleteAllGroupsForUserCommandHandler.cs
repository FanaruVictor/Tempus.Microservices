using GroupService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Tempus.Shared.Commons;

namespace GroupService.Infrastructure.Commands.Groups.DeleteAllGroupsForUser
{
    public class DeleteAllGroupsForUserCommandHandler(GroupServiceDbContext context, IConnection messageConnection) : IRequestHandler<DeleteAllGroupsForUserCommand, BaseResponse<bool>>
    {
        private readonly GroupServiceDbContext context = context;
        private readonly IConnection messageConnection = messageConnection;

        public async Task<BaseResponse<bool>> Handle(DeleteAllGroupsForUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Guid userId = Guid.Empty;

                if (!Guid.TryParse(request.UserId, out userId))
                {
                    return BaseResponse<bool>.NotFound("User not found");
                }


                var groups = await context
                    .Groups
                    .AsNoTracking()
                    .Where(x => x.OwnerId == userId)
                    .ToListAsync(cancellationToken);

                if (groups.Any())
                {
                    foreach (var group in groups)
                    {
                        var users = await context
                            .UserGroups
                            .AsNoTracking()
                            .Where(x => x.GroupId == group.Id)
                            .ToListAsync(cancellationToken);

                        context.RemoveRange(users);
                    }

                    context.RemoveRange(groups);
                }

                var userGroups = await context
                        .UserGroups
                        .AsNoTracking()
                        .Where(x => x.UserId == userId)
                        .ToListAsync(cancellationToken);

                context.RemoveRange(userGroups);


                await context.SaveChangesAsync(cancellationToken);

                foreach (var group in groups)
                {
                    SendDeleteGroupMessage(group.Id);
                }

                return BaseResponse<bool>.Ok(true);
            }
            catch (Exception exception)
            {
                return BaseResponse<bool>.BadRequest(new List<string> { exception.Message });
            }
        }

        private void SendDeleteGroupMessage(Guid groupId)
        {
            using var channel = messageConnection.CreateModel();

            channel.ExchangeDeclare(
                exchange: "deleteGroupExchange",
                ExchangeType.Fanout
            );

            channel.BasicPublish(
                exchange: "deleteGroupExchange",
                routingKey: "",
                basicProperties: null,
                body: System.Text.Encoding.UTF8.GetBytes(groupId.ToString())
            );
        }
    }
}
