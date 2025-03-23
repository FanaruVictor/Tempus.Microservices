using APIGateway.IServices;
using APIGateway.Models.Registrations;

namespace APIGateway.Services
{
    public class RegistrationService(IConfiguration configuration) : IRegistrationService
    {
        private readonly string registrationServiceBaseUrl = configuration["registrationServiceBaseURL"];

        public Task<RegistrationDetails> Create(Guid userId, NewRegistration newRegistration)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Delete(Guid userId, Guid id, Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RegistrationDetails>> GetAll(Guid userId, Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationDetails> GetById(Guid userId, Guid id, Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationDetails> GetLastUpdated(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationDetails> Update(Guid userId, RegistrationInfo registrationInfo)
        {
            throw new NotImplementedException();
        }
    }
}
