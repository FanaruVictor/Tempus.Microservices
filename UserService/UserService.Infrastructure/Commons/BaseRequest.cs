using MediatR;
using Newtonsoft.Json;

namespace UserService.Infrastructure.Commons;

public class BaseRequest<TResponse> : IRequest<TResponse>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}