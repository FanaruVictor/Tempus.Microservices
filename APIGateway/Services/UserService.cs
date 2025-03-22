using APIGateway.IServices;
using APIGateway.Models;
using Newtonsoft.Json;
using System.Text;

namespace APIGateway.Services
{
    public class UserService : IUserService
    {
        private readonly string userServiceBaseUrl;

        public UserService(IConfiguration configuration)
        {
            this.userServiceBaseUrl = configuration["userServiceBaseURL"];
        }
        public async Task<UserDetails> ChangeTheme(bool isDarkTheme, Guid id)
        {
            var json = JsonConvert.SerializeObject(new { IsDarkTheme = isDarkTheme });

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this.userServiceBaseUrl}/changeTheme");

            request.Headers.Add("UserId", id.ToString());
            request.Content = content;

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseString);

            return response.Resource;
        }

        public async Task<Guid> Delete(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, this.userServiceBaseUrl);

            request.Headers.Add("UserId", id.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<Guid>>(responseString);

            return response.Resource;
        }

        public async Task<List<UserDetails>> GetAll(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.userServiceBaseUrl}");

            request.Headers.Add("UserId", id.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<UserDetails>>>(responseString);

            return response.Resource;
        }

        public async Task<UserDetails> GetById(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.userServiceBaseUrl}/{id}");

            request.Headers.Add("UserId", id.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseString);

            return response.Resource;
        }

        public async Task<List<UserEmail>> GetEmails(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.userServiceBaseUrl}/emails");

            request.Headers.Add("UserId", id.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<List<UserEmail>>>(responseString);

            return response.Resource;
        }

        public async Task<bool> GetTheme(Guid id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.userServiceBaseUrl}/theme");

            request.Headers.Add("UserId", id.ToString());

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

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

            using var stream = user.NewPhoto.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(user.NewPhoto.ContentType);

            content.Add(fileContent, "file", user.NewPhoto.FileName);

            var request = new HttpRequestMessage(HttpMethod.Put, $"{this.userServiceBaseUrl}");

            request.Headers.Add("UserId", id.ToString());
            request.Content = content;

            using var httpClient = new HttpClient();

            var responseObject = await httpClient.SendAsync(request);

            var responseString = await responseObject.Content.ReadAsStringAsync();

            var response = JsonConvert.DeserializeObject<HttpResponse<UserDetails>>(responseString);

            return response.Resource;
        }
    }
}
