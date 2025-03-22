namespace APIGateway.Models
{
    public class HttpResponse<T>
    {
        public T? Resource { get; set; }

        public List<string> Errors { get; set; }
    }
}
