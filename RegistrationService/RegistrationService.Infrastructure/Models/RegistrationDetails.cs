namespace RegistrationService.Core.Models.Registrations;

public class RegistrationDetails
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public string CategoryColor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}