namespace APIGateway.Models.Registrations
{
    public class NewRegistration
    {
        public string? Description { get; init; }
        public string? Content { get; init; }
        public Guid CategoryId { get; init; }
    }
}
