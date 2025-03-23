using APIGateway.Models.Registrations;

namespace APIGateway.IServices
{
    public interface IRegistrationService
    {
        Task<List<RegistrationDetails>> GetAll(Guid userId, Guid groupId);
        Task<RegistrationDetails> GetById(Guid userId, Guid id, Guid groupId);
        Task<RegistrationDetails> Create(Guid userId, NewRegistration newRegistration);
        Task<RegistrationDetails> Update(Guid userId, RegistrationInfo registrationInfo);
        Task<Guid> Delete(Guid userId, Guid id, Guid groupId);
        Task<RegistrationDetails> GetLastUpdated(Guid userId);
    }
}
