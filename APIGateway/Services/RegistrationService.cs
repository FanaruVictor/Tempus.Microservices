using APIGateway.IServices;
using APIGateway.Models;
using APIGateway.Models.Registrations;
using Newtonsoft.Json;
using System.Text;
using Tempus.Shared.Models.Registration;

namespace APIGateway.Services
{
    public class RegistrationService(IHttpClientFactory httpClientFactory) : IRegistrationService
    {
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("registrationservice-api");


        public async Task<RegistrationDetails> Create(Guid userId, NewRegistration newRegistration, Guid groupId)
        {
            var json = JsonConvert.SerializeObject(new
            {
                newRegistration.Content,
                newRegistration.CategoryId,
                newRegistration.Description
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = groupId != Guid.Empty
                ? $"/api/registrations?groupId={groupId}"
                : "/api/registrations";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.PostAsync(url, content);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid userId, Guid id, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"/api/registrations/{id}?groupId={groupId}"
                : $"/api/registrations/{id}";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.DeleteAsync(url);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<RegistrationDetails>> GetAll(Guid userId, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"/api/registrations?groupId={groupId}"
                : "/api/registrations";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.GetAsync(url);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<RegistrationDetails>>>(responseString);

            return response.Resource;
        }

        public async Task<RegistrationDetails> GetById(Guid userId, Guid id, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"/api/registrations/{id}?groupId={groupId}"
                : $"/api/registrations/{id}";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.GetAsync(url);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }

        public async Task<RegistrationDetails> GetLastUpdated(Guid userId, Guid? groupId)
        {
            var url = groupId.HasValue && groupId.Value != Guid.Empty
                ? $"/api/registrations/lastUpdated?groupId={groupId}"
                : $"/api/registrations/lastUpdated";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.GetAsync(url);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }

        public async Task<RegistrationDetails> Update(Guid userId, RegistrationInfo registrationInfo, Guid groupId)
        {
            var json = JsonConvert.SerializeObject(new
            {
                registrationInfo.Id,
                registrationInfo.Content,
                registrationInfo.Description,
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = groupId != Guid.Empty
                ? $"/api/registrations?groupId={groupId}"
                : $"/api/registrations";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.PutAsync(url, content);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }
    }
}
