using APIGateway.IServices;
using APIGateway.Models;
using APIGateway.Models.Group;
using Newtonsoft.Json;
using Tempus.Shared.Models.Group;

namespace APIGateway.Services
{
    public class GroupService(IConfiguration configuration) : IGroupService
    {
        private readonly string groupServiceBaseUrl = configuration["groupServiceBaseURL"];

        public async Task<bool> Create(Guid userId, NewGroup newGroup)
        {
            var formData = new Dictionary<string, string>
            {
                {"Name", newGroup.Name},
                {"Members", newGroup.Members}
            };

            using var content = new MultipartFormDataContent();

            foreach(var field in formData)
            {
                content.Add(new StringContent(field.Value), field.Key);
            }

            if(newGroup.Image != null)
            {
                using var stream = newGroup.Image.OpenReadStream();

                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(newGroup.Image.ContentType);

                content.Add(fileContent, "file", newGroup.Image.FileName);
            }


            var request = new HttpRequestMessage(HttpMethod.Post, $"{this.groupServiceBaseUrl}");

            request.Content = content;
            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<bool>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid userId, Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{this.groupServiceBaseUrl}/{id}");

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<GroupOverview>> GetAll(Guid userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, this.groupServiceBaseUrl);

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<GroupOverview>>>(responseString);

            return response.Resource;
        }

        public async Task<GroupDetails> GetById(Guid userId, Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.groupServiceBaseUrl}/{id}");

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<GroupDetails>>(responseString);

            return response.Resource;
        }

        public async Task<GroupOverview> Update(Guid userId, GroupInfo groupInfo)
        {
            var formData = new Dictionary<string, string>
            {
                {"Id", groupInfo.Id.ToString()},
                {"Name", groupInfo.Name},
                {"Members", groupInfo.Members},
                {"IsCurrentImageChanged", groupInfo.IsCurrentImageChanged.ToString()}
            };

            using var content = new MultipartFormDataContent();

            foreach(var field in formData)
            {
                content.Add(new StringContent(field.Value), field.Key);
            }

            if(groupInfo.Image != null)
            {
                using var stream = groupInfo.Image.OpenReadStream();

                var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(groupInfo.Image.ContentType);

                content.Add(fileContent, "file", groupInfo.Image.FileName);
            }


            var request = new HttpRequestMessage(HttpMethod.Put, $"{this.groupServiceBaseUrl}");

            request.Content = content;
            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<GroupOverview>>(responseString);

            return response.Resource;
        }
    }
}