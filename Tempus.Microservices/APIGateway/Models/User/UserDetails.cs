namespace APIGateway.Models.User
{
    public class UserDetails
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDarkTheme { get; set; }
        public Photo Photo { get; set; }
    }
}
