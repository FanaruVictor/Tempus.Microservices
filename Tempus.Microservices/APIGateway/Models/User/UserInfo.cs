namespace APIGateway.Models.User
{
    public class UserInfo
    {
        public string UserName { get; init; }
        public string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public bool IsPhotoChanged { get; set; }
        public IFormFile? NewPhoto { get; set; }
    }
}
