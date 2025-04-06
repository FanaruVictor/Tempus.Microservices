namespace GroupService.Core.Entities;

public class Group
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Photo GroupPhoto { get; set; }
}