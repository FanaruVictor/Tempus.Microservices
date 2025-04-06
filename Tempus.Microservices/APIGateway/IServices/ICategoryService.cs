using APIGateway.Models.Category;

namespace APIGateway.IServices
{
    public interface ICategoryService
    {
        Task<List<BaseCategory>> GetAll(Guid userId, Guid groupId);
        Task<BaseCategory> GetById(Guid userId, Guid id, Guid groupId);

        Task<BaseCategory> Create(Guid userId, NewCategory newCategory, Guid groupId);
        Task<BaseCategory> Update(Guid userId, CategoryInfo categoryInfo, Guid groupId);
        Task<Guid> Delete(Guid userId, Guid id, Guid groupId);
    }
}
