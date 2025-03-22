namespace RegistrationService.Core.Models.Registrations;

public class BaseRegistration
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public string Description { get; set; }
    public string CategoryColor { get; set; }

}