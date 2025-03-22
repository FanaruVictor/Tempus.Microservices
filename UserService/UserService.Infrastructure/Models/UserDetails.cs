namespace UserService.Infrastructure.Models;

public class UserDetails
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public PhotoDetails Photo { get; set; }
    public bool IsDarkTheme { get; set; }
}