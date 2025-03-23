using APIGateway.IServices;
using APIGateway.Models;
using APIGateway.Models.Category;
using Newtonsoft.Json;
using System.Text;

namespace APIGateway.Services
{
    public class CategoryService(IConfiguration configuration) : ICategoryService
    {
        private readonly string categoryServiceBaseUrl = configuration["categoryServiceBaseURL"];

        public async Task<BaseCategory> Create(Guid userId, NewCategory newCategory)
        {
            var json = JsonConvert.SerializeObject(new
            {
                newCategory.Name,
                newCategory.Color,
                newCategory.GroupId,
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{this.categoryServiceBaseUrl}");

            request.Headers.Add("UserId", userId.ToString());
            request.Content = content;

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<BaseCategory>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid userId, Guid id, Guid groupId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{this.categoryServiceBaseUrl}/{id}?groupId={groupId}");

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<BaseCategory>> GetAll(Guid userId, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"{this.categoryServiceBaseUrl}?groupId={groupId}"
                : this.categoryServiceBaseUrl;

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<BaseCategory>>>(responseString);

            return response.Resource;
        }

        public async Task<BaseCategory> GetById(Guid userId, Guid id, Guid groupId)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.categoryServiceBaseUrl}/{id}?groupId={groupId}");

            request.Headers.Add("UserId", userId.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<BaseCategory>>(responseString);

            return response.Resource;
        }

        public async Task<BaseCategory> Update(Guid userId, CategoryInfo categoryInfo, Guid groupId)
        {
            var json = JsonConvert.SerializeObject(new
            {
                categoryInfo.Id,
                categoryInfo.Name,
                categoryInfo.Color,
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this.categoryServiceBaseUrl}?groupId={groupId}");

            request.Headers.Add("UserId", userId.ToString());
            request.Content = content;

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<BaseCategory>>(responseString);

            return response.Resource;
        }
    }
}
