using GroupService.Core.Commons;
using GroupService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Infrastructure.Commands.Groups.Delete;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, BaseResponse<Guid>>
{
    private readonly GroupServiceDbContext context;

    public DeleteGroupCommandHandler(GroupServiceDbContext context)
    {
        this.context = context;
    }

    public async Task<BaseResponse<Guid>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var group = await this.context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (group == null)
            {
                return BaseResponse<Guid>.NotFound($"Group with id: {request.Id} not found");
            }

            if (group.OwnerId == request.UserId)
            {
                this.context.Groups.Remove(group);

                await RemoveAllMembers(group.Id);
            }
            else
            {
                var userGroup = await this.context.UserGroups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.GroupId == group.Id);

                if (userGroup == null)
                {
                    return BaseResponse<Guid>.NotFound($"User with id: {request.UserId} not found");
                }

                this.context.UserGroups.Remove(userGroup);
            }

            await this.context.SaveChangesAsync(cancellationToken);

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
        var userGroups = await this.context.UserGroups
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .ToListAsync();

        this.context.UserGroups.RemoveRange(userGroups);
    }
}