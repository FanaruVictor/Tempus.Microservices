using APIGateway.Models.User;

namespace APIGateway.Models.Group;

public class GroupDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public List<UserEmail> Members { get; set; }
}