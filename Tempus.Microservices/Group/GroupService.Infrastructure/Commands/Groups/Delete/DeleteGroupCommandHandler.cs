using GroupService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Tempus.Shared.Commons;

namespace GroupService.Infrastructure.Commands.Groups.Delete;

public class DeleteGroupCommandHandler(GroupServiceDbContext context, IConnection messageConnection) : IRequestHandler<DeleteGroupCommand, BaseResponse<Guid>>
{
    private readonly GroupServiceDbContext context = context;
    private readonly IConnection messageConnection = messageConnection;

    public async Task<BaseResponse<Guid>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var group = await context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (group == null)
            {
                return BaseResponse<Guid>.NotFound($"Group with id: {request.Id} not found");
            }

            if (group.OwnerId == request.UserId)
            {
                context.Groups.Remove(group);

                await RemoveAllMembers(group.Id);
            }
            else
            {
                var userGroup = await context.UserGroups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.GroupId == group.Id);

                if (userGroup == null)
                {
                    return BaseResponse<Guid>.NotFound($"User with id: {request.UserId} not found");
                }

                context.UserGroups.Remove(userGroup);
            }

            await context.SaveChangesAsync(cancellationToken);

            SendDeleteGroupMessage(group.Id);

            return BaseResponse<Guid>.Ok(group.Id);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private async Task RemoveAllMembers(Guid groupId)
    {
        var userGroups = await context.UserGroups
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .ToListAsync();

        context.UserGroups.RemoveRange(userGroups);
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