using CloudinaryDotNet.Actions;
using MediatR;
using Microsoft.AspNetCore.Http;
using GroupService.Core.Commons;
using GroupService.Core.Entities.Group;
using GroupService.Core.Entities.User;
using GroupService.Core.IRepositories;
using GroupService.Core.Models.Group;
using GroupService.Infrastructure.Commons;
using GroupService.Infrastructure.Services.Cloudynary;
using StatusCodes = GroupService.Core.Commons.StatusCodes;

namespace GroupService.Infrastructure.Commands.Groups.Update;

public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, BaseResponse<GroupOverview>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupUserRepository _groupUserRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IGroupPhotoRepository _groupPhotoRepository;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository, IGroupUserRepository groupUserRepository, ICloudinaryService cloudinaryService, IGroupPhotoRepository groupPhotoRepository)
    {
        _groupRepository = groupRepository;
        _groupUserRepository = groupUserRepository;
        _cloudinaryService = cloudinaryService;
        _groupPhotoRepository = groupPhotoRepository;
    }
    
    public async Task<BaseResponse<GroupOverview>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var group = await _groupRepository.GetById(request.Id);

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
        
            _groupRepository.Update(entity);

            var groupMembers = await _groupRepository.GetGroupMembers(group.Id);
            groupMembers = groupMembers.Where(x => x.Id != request.UserId).ToList();

            await UpdateMembers(request, groupMembers, group);

            if(request.IsCurrentImageChanged)
            {
                var updatePhotoResult =  await UpdatePhoto(request.Image, group);
                
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
            
            await _groupRepository.SaveChanges();

            var groupOverview =  new GroupOverview
            {
                Id = group.Id,
                Name = group.Name,
                Image = group.GroupPhoto?.Url,
                UserCount = _groupRepository.GetUserCount(group.Id),
                CreatedAt = group.CreatedAt,
                OwnerId = group.OwnerId
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

    private async Task UpdateMembers(UpdateGroupCommand request, List<User> groupMembers, Group group)
    {
        var groupMembersIds = groupMembers.Select(x => x.Id.ToString()).ToList();
        request.Members = request.Members.Substring(1, request.Members.Length - 2);
        var members = request.Members.Split(",").ToList();

        var newMembers = members.Where(x => !groupMembersIds.Contains(x));
        var removedMembers = groupMembersIds.Where(x => !members.Contains(x));

        var newGroupMembers = newMembers.Select(x => new GroupUser
        {
            UserId = Guid.Parse(x),
            GroupId = group.Id
        }).ToList();
        
        await _groupUserRepository.AddRange(newGroupMembers);

        var removeGroupMembers = removedMembers.Select(x => new GroupUser
        {
            UserId = Guid.Parse(x),
            GroupId = group.Id
        }).ToList();

        await _groupUserRepository.RemoveRange(removeGroupMembers);
    }

    private async Task<BaseResponse<GroupPhoto>> UpdatePhoto(IFormFile? photo, Group group)
    {
        if(photo == null)
        {
            if(group.GroupPhoto != null)
            {
                await _cloudinaryService.DestroyUsingGroupId(group.Id);
                await _groupPhotoRepository.Delete(group.GroupPhoto.Id);
            }
            
            return BaseResponse<GroupPhoto>.Ok();
        }

        ImageUploadResult uploadResult;
        GroupPhoto groupPhoto;

        if(group.GroupPhoto != null)
        {
            await _cloudinaryService.DestroyUsingGroupId(group.Id);
            uploadResult = await _cloudinaryService.Upload(photo);
            groupPhoto = new GroupPhoto
            {
                Id = group.GroupPhoto.Id,
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                GroupId = group.Id
            };
            _groupPhotoRepository.Update(groupPhoto);
        }
        else
        {
            uploadResult = await _cloudinaryService.Upload(photo);
            groupPhoto = new GroupPhoto
            {
                Id = Guid.NewGuid(),
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString(),
                GroupId = group.Id
            };
            await _groupPhotoRepository.Add(groupPhoto);
        }


        return BaseResponse<GroupPhoto>.Ok(groupPhoto);
    }
}