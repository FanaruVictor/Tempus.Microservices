using GroupService.Core.Commons;
using MediatR;

namespace GroupService.Infrastructure.Queries.Group.GetAllGroupsQuery;

public class GetAllGroupsQueryHandler : IRequestHandler<GetAllGroupsQuery, BaseResponse<List<GroupOverview>>>
{
    private readonly IGroupRepository _groupRepository;

    public GetAllGroupsQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<BaseResponse<List<GroupOverview>>> Handle(GetAllGroupsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var groups = await _groupRepository.GetAll(request.UserId);

            var groupsOverview = groups.Select(x =>
            {
                var groupImage = x.GroupPhoto?.Url;

                return new GroupOverview
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = groupImage,
                    UserCount = _groupRepository.GetUserCount(x.Id),
                    CreatedAt = x.CreatedAt,
                    OwnerId = x.OwnerId
                };
            }).ToList();


            foreach (var group in groupsOverview)
            {
                var currentUserPhotos = await _groupRepository.GetUsersPhoto(group.Id);
                group.UserPhotos = currentUserPhotos;
            }

            return BaseResponse<List<GroupOverview>>.Ok(groupsOverview);
        }
        catch (Exception exception)
        {
            return BaseResponse<List<GroupOverview>>.BadRequest(new List<string> { exception.Message });
        }
    }
}