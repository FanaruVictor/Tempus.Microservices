namespace GroupService.Core.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public string PublicId { get; set; }
    public string Url { get; set; }
    public Guid GroupId { get; set; }
    public Group Group { get; set; }

}