using MediatR;
using Newtonsoft.Json;

namespace CategoryService.Infrastructure.Commons;

public class BaseRequest<TResponse> : IRequest<TResponse>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}