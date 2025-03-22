namespace GroupService.Infrastructure.Models;

public class RegistrationOverview
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public string CategoryColor { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}