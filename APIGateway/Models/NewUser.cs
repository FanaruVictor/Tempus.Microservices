namespace APIGatewat.Models;

public class NewUser
{
    public string UserName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
