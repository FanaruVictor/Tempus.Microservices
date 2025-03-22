using MediatR;
using GroupService.Core.Commons;
using GroupService.Core.IRepositories;
using GroupService.Core.Models.Group;
using GroupService.Core.Models.User;
using GroupService.Infrastructure.Commons;

namespace GroupService.Infrastructure.Queries.Group.GetGroupByIdQuery;

public class GetGroupByIdQueryHandler : IRequestHandler<GetGroupByIdQuery, BaseResponse<GroupDetails>>
{
    private readonly IGroupRepository _groupRepository;
    private IUserRepository _userRepository;

    public GetGroupByIdQueryHandler(IGroupRepository groupRepository, IUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<GroupDetails>> Handle(GetGroupByIdQuery request,
        CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetById(request.Id);

        if(group == null)
        {
            return BaseResponse<GroupDetails>.NotFound("Group not found!");
        }

        var validator = ValidateGroup(request, group);

        if(validator.StatusCode != StatusCodes.Ok)
        {
            return validator;
        }

        var groupDetails = validator.Resource;
        
        groupDetails.Image = group.GroupPhoto?.Url;
        
        var usersId = await _groupRepository.GetUsers(group.Id);
        
        var userEmails = (await _userRepository.GetAll())
            .Where(x => usersId.Contains(x.Id) && x.Id != request.UserId)
            .Select(x => new UserEmail
                {
                    Email = x.Email,
                    Id = x.Id,
                    PhotoUrl = x.UserPhoto?.Url
                })
            .ToList();

        groupDetails.Members = userEmails;
        
        var result = BaseResponse<GroupDetails>.Ok(groupDetails);
        return result;
    }

    private BaseResponse<GroupDetails> ValidateGroup(GetGroupByIdQuery request, Core.Entities.Group.Group group)
    {
        if(request.UserId != group.OwnerId)
        {
            return BaseResponse<GroupDetails>.Forbbiden();
        }

        return BaseResponse<GroupDetails>.Ok(GenericMapper<Core.Entities.Group.Group, GroupDetails>.Map(group));
    }
}