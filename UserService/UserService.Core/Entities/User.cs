namespace UserService.Core.Entities
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDarkTheme { get; set; }
        public string? ExternalId { get; set; }
        public UserPhoto UserPhoto { get; set; }
    }
}
