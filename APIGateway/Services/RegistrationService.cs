using APIGateway.IServices;
using APIGateway.Models;
using APIGateway.Models.Registrations;
using Newtonsoft.Json;
using System.Text;
using Tempus.Shared.Models.Registration;

namespace APIGateway.Services
{
    public class RegistrationService(IConfiguration configuration) : IRegistrationService
    {
        private readonly string registrationServiceBaseUrl = configuration["registrationServiceBaseURL"];

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
                ? $"{this.registrationServiceBaseUrl}?groupId={groupId}"
                : $"{this.registrationServiceBaseUrl}";

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Add("UserId", userId.ToString());
            request.Content = content;

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid userId, Guid id, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"{this.registrationServiceBaseUrl}/{id}?groupId={groupId}"
                : $"{this.registrationServiceBaseUrl}/{id}";

            var request = new HttpRequestMessage(HttpMethod.Delete, url);

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<RegistrationDetails>> GetAll(Guid userId, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"{this.registrationServiceBaseUrl}?groupId={groupId}"
                : this.registrationServiceBaseUrl;

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<RegistrationDetails>>>(responseString);

            return response.Resource;
        }

        public async Task<RegistrationDetails> GetById(Guid userId, Guid id, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"{this.registrationServiceBaseUrl}/{id}?groupId={groupId}"
                : $"{this.registrationServiceBaseUrl}/{id}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }

        public async Task<RegistrationDetails> GetLastUpdated(Guid userId, Guid? groupId)
        {
            var url = groupId.HasValue && groupId.Value != Guid.Empty
                ? $"{this.registrationServiceBaseUrl}/lastUpdated?groupId={groupId}"
                : $"{this.registrationServiceBaseUrl}/lastUpdated";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

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
                ? $"{this.registrationServiceBaseUrl}?groupId={groupId}"
                : $"{this.registrationServiceBaseUrl}";

            var request = new HttpRequestMessage(HttpMethod.Put, url);

            request.Headers.Add("UserId", userId.ToString());
            request.Content = content;

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<RegistrationDetails>>(responseString);

            return response.Resource;
        }
    }
}
