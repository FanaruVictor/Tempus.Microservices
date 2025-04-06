using APIGateway.Models.Group;

namespace APIGateway.IServices
{
    public interface IGroupService
    {
        Task<bool> Create(Guid userId, NewGroup newGroup);

        Task<List<GroupOverview>> GetAll(Guid userId);

        Task<GroupDetails> GetById(Guid userId, Guid id);

        Task<GroupOverview> Update(Guid userId, GroupInfo groupInfo);

        Task<Guid> Delete(Guid userId, Guid id);
    }
}
