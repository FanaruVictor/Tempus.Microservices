namespace APIGateway.Models.Category
{
    public class BaseCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? Color { get; set; }
    }
}
