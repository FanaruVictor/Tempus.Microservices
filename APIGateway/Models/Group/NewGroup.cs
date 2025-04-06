namespace APIGateway.Models.Group
{
    public class NewGroup
    {
        public string Name { get; set; }
        public string? Members { get; set; }
        public IFormFile? Image { get; set; }
    }
}
