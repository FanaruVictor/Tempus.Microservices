using MediatR;
using GroupService.Core.Commons;
using GroupService.Core.Entities;
using GroupService.Core.Entities.Group;
using GroupService.Core.IRepositories;
using GroupService.Infrastructure.Services.Cloudynary;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, BaseResponse<bool>>
{
    private readonly IGroupRepository _groupRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IGroupUserRepository _groupUserRepository;
    private readonly IGroupPhotoRepository _groupPhotoRepository;

    public CreateGroupCommandHandler(IGroupRepository groupRepository, ICloudinaryService cloudinaryService,
         IGroupUserRepository groupUserRepository, IGroupPhotoRepository groupPhotoRepository)
    {
        _groupRepository = groupRepository;
        _cloudinaryService = cloudinaryService;
        _groupUserRepository = groupUserRepository;
        _groupPhotoRepository = groupPhotoRepository;
    }

    public async Task<BaseResponse<bool>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                OwnerId = request.UserId,
                CreatedAt = DateTime.UtcNow 
            };

            await _groupRepository.Add(group);

            await AddImage(request, group.Id);

            await AddGroupUser(request, group.Id);

            await _groupRepository.SaveChanges();

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
        var groupUsers = new List<GroupUser>
        {
            new GroupUser
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
            groupUsers.Add(new GroupUser
            {
                GroupId = groupId,
                UserId = new Guid(member)
            });
        }

        await _groupUserRepository.AddRange(groupUsers);
    }

    private async Task AddImage(CreateGroupCommand request, Guid groupId)
    {
        if(request.Image == null)
        {
            return;
        }

        var uploadResult = await _cloudinaryService.Upload(request.Image);

        var groupPhoto = new GroupPhoto
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            PublicId = uploadResult.PublicId,
            Url = uploadResult.Url.ToString(),
        };

        await _groupPhotoRepository.Add(groupPhoto);
    }
}