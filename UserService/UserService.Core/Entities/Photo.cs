namespace UserService.Core.Entities
{
    public sealed class Photo
    {
        public Guid Id { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
