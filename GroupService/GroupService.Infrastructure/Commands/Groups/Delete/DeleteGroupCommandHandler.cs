using MediatR;
using GroupService.Core.Commons;
using GroupService.Core.IRepositories;

namespace GroupService.Infrastructure.Commands.Groups.Delete;

public class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand, BaseResponse<Guid>>
{
    private readonly IGroupRepository _groupRepository;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<BaseResponse<Guid>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var group = await _groupRepository.GetById(request.Id);

            if (group == null)
            {
                return BaseResponse<Guid>.NotFound($"Group with id: {request.Id} not found");
            }

            if(group.OwnerId == request.UserId)
            {
                await _groupRepository.Delete(group.Id);
            }
            else
            {
                var groupUser = await _groupRepository.GetGroupUser(request.UserId, group.Id);

                if (groupUser == null)
                {
                    return BaseResponse<Guid>.NotFound($"User with id: {request.UserId} not found");
                }

                await _groupRepository.DeleteGroupMember(request.UserId, group.Id);
            }

            await _groupRepository.SaveChanges();

            return BaseResponse<Guid>.Ok(group.Id);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }
}