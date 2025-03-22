namespace RegistrationService.Core.Entities;

public class Registration
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public Guid CategoryId { get; set; }
}