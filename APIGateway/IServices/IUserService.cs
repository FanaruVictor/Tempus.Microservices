using APIGateway.Models.User;

namespace APIGateway.IServices
{
    public interface IUserService
    {
        Task<List<UserDetails>> GetAll(Guid id);
        Task<UserDetails> GetById(Guid id);
        Task<UserDetails> Update(Guid id, UserInfo user);
        Task<Guid> Delete(Guid id);
        Task<UserDetails> ChangeTheme(bool isDarkTheme, Guid id);
        Task<bool> GetTheme(Guid id);
        Task<List<UserEmail>> GetEmails(Guid id);
    }
}
