using APIGateway.IServices;
using APIGateway.Models;
using APIGateway.Models.Category;
using Newtonsoft.Json;
using System.Text;
using Tempus.Shared.Models.Category;

namespace APIGateway.Services
{
    public class CategoryService(IHttpClientFactory httpClientFactory) : ICategoryService
    {
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("categoryservice-api");

        public async Task<BaseCategory> Create(Guid userId, NewCategory newCategory, Guid groupId)
        {
            var json = JsonConvert.SerializeObject(new
            {
                newCategory.Name,
                newCategory.Color,
            });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = groupId != Guid.Empty
                ? $"/api/categories?groupId={groupId}"
                : "/api/categories";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.PostAsync(url, content);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<BaseCategory>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid userId, Guid id, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"/api/categories/{id}?groupId={groupId}"
                : $"/api/categories/{id}";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.DeleteAsync(url);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<BaseCategory>> GetAll(Guid userId, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"/api/categories/?groupId={groupId}"
                : "/api/categories";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.GetAsync(url);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<BaseCategory>>>(responseString);

            return response.Resource;
        }

        public async Task<BaseCategory> GetById(Guid userId, Guid id, Guid groupId)
        {
            var url = groupId != Guid.Empty
                ? $"/api/categories/{id}?groupId={groupId}"
                : $"/api/categories/{id}";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.GetAsync(url);

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

            var url = groupId != Guid.Empty
                ? $"/api/categories?groupId={groupId}"
                : "/api/categories";

            httpClient.DefaultRequestHeaders.Add("UserId", userId.ToString());

            var responseObject = await httpClient.PutAsync(url, content);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<BaseCategory>>(responseString);

            return response.Resource;
        }
    }
}
