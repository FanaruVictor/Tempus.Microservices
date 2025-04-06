namespace APIGateway.Models.Group
{
    public class GroupInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Members { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsCurrentImageChanged { get; set; }
    }
}
