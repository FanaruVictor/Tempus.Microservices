using APIGateway.IServices;
using APIGateway.Models;
using APIGateway.Models.User;
using Newtonsoft.Json;
using System.Text;
using Tempus.Shared.Models.User;

namespace APIGateway.Services
{
    public class UserService(IHttpClientFactory httpClientFactory) : IUserService
    {
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("userservice-api");

        public async Task<UserDetails> ChangeTheme(bool isDarkTheme, Guid id)
        {
            var json = JsonConvert.SerializeObject(new { IsDarkTheme = isDarkTheme });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.PutAsync("/api/users/changeTheme", content);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid id)
        {
            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.DeleteAsync("/api/users");

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<UserDetails>> GetAll(Guid id)
        {
            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.GetAsync("/api/users");

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<UserDetails>>>(responseString);

            return response.Resource;
        }

        public async Task<UserDetails> GetById(Guid id)
        {
            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.GetAsync($"/api/users/{id}");

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseString);

            return response.Resource;
        }

        public async Task<List<UserEmail>> GetEmails(Guid id)
        {
            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.GetAsync("/api/users/emails");

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<UserEmail>>>(responseString);

            return response.Resource;
        }

        public async Task<bool> GetTheme(Guid id)
        {
            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.GetAsync("/api/users/theme");

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<bool>>(responseString);

            return response.Resource;
        }

        public async Task<UserDetails> Update(Guid id, UserInfo user)
        {
            var formData = new Dictionary<string, string>
            {
                {"UserName", user.UserName },
                {"Email", user.Email },
                {"PhoneNumber", user.PhoneNumber },
                {"IsPhotoChanged", user.IsPhotoChanged.ToString() },
            };

            using var content = new MultipartFormDataContent();

            foreach (var field in formData)
            {
                content.Add(new StringContent(field.Value), field.Key);
            }

            if (user.IsPhotoChanged == true && user.NewPhoto != null)
            {
                using var stream = user.NewPhoto.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(user.NewPhoto.ContentType);

                content.Add(fileContent, "file", user.NewPhoto.FileName);
            }

            httpClient.DefaultRequestHeaders.Add("UserId", id.ToString());

            var responseObject = await httpClient.PutAsync("/api/users", content);

            responseObject.EnsureSuccessStatusCode();

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseString);

            return response.Resource;
        }
    }
}
