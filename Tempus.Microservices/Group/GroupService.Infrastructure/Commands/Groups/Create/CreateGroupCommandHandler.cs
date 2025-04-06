using GroupService.Core.Entities;
using GroupService.Data.Context;
using MediatR;
using Tempus.Shared.Commons;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, BaseResponse<bool>>
{
    //private readonly ICloudinaryService _cloudinaryService;
    private readonly GroupServiceDbContext context;

    public CreateGroupCommandHandler(GroupServiceDbContext context)
    {
        //_cloudinaryService = cloudinaryService;
        this.context = context;
    }

    public async Task<BaseResponse<bool>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(request.UserId == null)
            {
                return BaseResponse<bool>.BadRequest(new List<string>());
            }

            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                OwnerId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            await context.Groups.AddAsync(group);

            //await AddImage(request, group.Id);

            await AddGroupUser(request, group.Id);

            await context.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.Ok(true);
        }
        catch(Exception exception)
        {
            return BaseResponse<bool>.BadRequest(new List<string>
            {
                exception.Message
            });
        }
    }

    private async Task AddGroupUser(CreateGroupCommand request, Guid groupId)
    {
        var groupUsers = new List<UserGroup>
        {
            new()
            {
                GroupId = groupId,
                UserId = request.UserId
            }
        };

        var members = request.Members
            .Replace("\"", "")
            .Split(',')
            .Select(x => x.Replace("\"", ""));

        foreach(var member in members)
        {
            groupUsers.Add(new UserGroup
            {
                GroupId = groupId,
                UserId = new Guid(member)
            });
        }

        await context.UserGroups.AddRangeAsync(groupUsers);
    }

    private async Task AddImage(CreateGroupCommand request, Guid groupId)
    {
        if(request.Image == null) { }

        /*var uploadResult = await _cloudinaryService.Upload(request.Image);

         var groupPhoto = new Photo
         {
             Id = Guid.NewGuid(),
             GroupId = groupId,
             PublicId = uploadResult.PublicId,
             Url = uploadResult.Url.ToString(),
         };

         await this.context.Photos.AddAsync(groupPhoto);*/
    }
}