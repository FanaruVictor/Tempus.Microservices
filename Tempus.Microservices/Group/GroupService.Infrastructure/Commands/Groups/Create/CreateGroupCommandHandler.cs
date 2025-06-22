using GroupService.Core.Entities;
using GroupService.Data.Context;
using MediatR;
using Tempus.Shared.Commons;
using Tempus.Shared.Models.Group;

namespace GroupService.Infrastructure.Commands.Groups.Create;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, BaseResponse<GroupOverview>>
{
	//private readonly ICloudinaryService _cloudinaryService;
	private readonly GroupServiceDbContext context;

	public CreateGroupCommandHandler (GroupServiceDbContext context)
	{
		//_cloudinaryService = cloudinaryService;
		this.context = context;
	}

	public async Task<BaseResponse<GroupOverview>> Handle (CreateGroupCommand request, CancellationToken cancellationToken)
	{
		try
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (request.UserId == null)
			{
				return BaseResponse<GroupOverview>.BadRequest(new List<string>());
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

			var groupOverview = GenericMapper<Group, GroupOverview>.Map(group);

			return BaseResponse<GroupOverview>.Ok(groupOverview);
		}
		catch (Exception exception)
		{
			return BaseResponse<GroupOverview>.BadRequest(new List<string>
			{
				exception.Message
			});
		}
	}

	private async Task AddGroupUser (CreateGroupCommand request, Guid groupId)
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

		members = members.Where(x => x != request.UserId.ToString());

		foreach (var member in members)
		{
			groupUsers.Add(new UserGroup
			{
				GroupId = groupId,
				UserId = new Guid(member)
			});
		}

		await context.UserGroups.AddRangeAsync(groupUsers);
	}

	private async Task AddImage (CreateGroupCommand request, Guid groupId)
	{
		if (request.Image == null)
		{ }

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