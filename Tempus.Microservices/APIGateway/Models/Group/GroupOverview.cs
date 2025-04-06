namespace APIGateway.Models.Group;

public class GroupOverview
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public List<string> UserPhotos { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid OwnerId { get; set; }
}