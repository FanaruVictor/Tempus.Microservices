namespace UserService.Core.Entities
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDarkTheme { get; set; }
        public string Password { get; set; }
        public Photo Photo { get; set; }
        public List<Guid> GroupIds { get; set; }
    }
}
