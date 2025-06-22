using GroupService.Core.Entities;
using GroupService.Data.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Commands.Groups.Update;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, BaseResponse<GroupOverview>>
{
    //private readonly ICloudinaryService cloudinaryService;
    private readonly GroupServiceDbContext context;

    public UpdateGroupCommandHandler(GroupServiceDbContext context)
    {
        //this.cloudinaryService = cloudinaryService;
        this.context = context;
    }

    public async Task<BaseResponse<GroupOverview>> Handle(UpdateGroupCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var group = await context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if(group == null)
            {
                return BaseResponse<GroupOverview>.NotFound("Group not found");
            }

            if(group.OwnerId != request.UserId)
            {
                return BaseResponse<GroupOverview>.Forbbiden();
            }

            var entity = new Group
            {
                Id = group.Id,
                OwnerId = group.OwnerId,
                Name = request.Name,
                CreatedAt = group.CreatedAt
            };

            context.Groups.Update(entity);

            var groupMembers = await context.UserGroups
                .AsNoTracking()
                .Where(x => x.GroupId == entity.Id)
                .ToListAsync();

            groupMembers = groupMembers.Where(x => x.UserId != request.UserId).ToList();

            await UpdateMembers(request, groupMembers, group);

            /*if (request.IsCurrentImageChanged)
            {
                var updatePhotoResult = await UpdatePhoto(request.Image, group);

                if (updatePhotoResult.StatusCode != StatusCodes.Ok)
                {
                    return new BaseResponse<GroupOverview>
                    {
                        Errors = updatePhotoResult.Errors,
                        StatusCode = updatePhotoResult.StatusCode,
                    };
                }

                group.GroupPhoto = updatePhotoResult.Resource ?? null;
            }
            */

            await context.SaveChangesAsync(cancellationToken);

            var groupOverview = new GroupOverview
            {
                Id = entity.Id,
                Name = entity.Name,
                Image = entity.GroupPhoto?.Url,
                UserCount = context.UserGroups.Count(x => x.GroupId == entity.Id),
                CreatedAt = entity.CreatedAt,
                OwnerId = entity.OwnerId
            };

            return BaseResponse<GroupOverview>.Ok(groupOverview);
        }
        catch(Exception e)
        {
            return BaseResponse<GroupOverview>.BadRequest(new List<string>
            {
                e.Message
            });
        }
    }

    private async Task UpdateMembers(UpdateGroupCommand request, List<UserGroup> groupMembers, Group group)
    {
        var groupMembersIds = groupMembers.Select(x => x.UserId.ToString()).ToList();

        List<string> members = new List<string>();

        if(request.Members != null)
        {
            members = request.Members.ToLower().Split(",").ToList();
        }

        var newMembers = members.Where(x => !groupMembersIds.Contains(x));
        var removedMembers = groupMembersIds.Where(x => !members.Contains(x));

        var newGroupMembers = newMembers.Select(x => new UserGroup
        {
            UserId = Guid.Parse(x),
            GroupId = group.Id
        }).ToList();

        await context.UserGroups.AddRangeAsync(newGroupMembers);

        var removeGroupMembers = removedMembers.Select(x => new UserGroup
        {
            UserId = Guid.Parse(x),
            GroupId = group.Id
        }).ToList();

        context.UserGroups.RemoveRange(removeGroupMembers);
    }

    /*private async Task<BaseResponse<Photo>> UpdatePhoto(IFormFile? photo, Group group)
    {
        if (photo == null)
        {
            if (group.GroupPhoto != null)
            {
                await this.cloudinaryService.DestroyUsingGroupId(group.Id);

                var dbPhoto = await this.context.Photos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == group.GroupPhoto.Id);

                this.context.Photos.Remove(dbPhoto);
            }

            return BaseResponse<Photo>.Ok();
        }

        ImageUploadResult uploadResult;
        Photo groupPhoto;

        if (group.GroupPhoto != null)
        {
            await this.cloudinaryService.DestroyUsingGroupId(group.Id);

            uploadResult = await this.cloudinaryService.Upload(photo);
            groupPhoto = new Photo
            {
                Id = group.GroupPhoto.Id,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                GroupId = group.Id
            };

            this.context.Photos.Update(groupPhoto);
        }
        else
        {
            uploadResult = await this.cloudinaryService.Upload(photo);
            groupPhoto = new Photo
            {
                Id = Guid.NewGuid(),
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                GroupId = group.Id
            };
            await this.context.Photos.AddAsync(groupPhoto);
        }


        return BaseResponse<Photo>.Ok(groupPhoto);
    }*/
}