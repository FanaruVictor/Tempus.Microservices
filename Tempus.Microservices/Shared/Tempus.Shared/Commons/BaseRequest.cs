using MediatR;
using Newtonsoft.Json;

namespace Tempus.Shared.Commons;

public class BaseRequest<TResponse> : IRequest<TResponse>
{
    [JsonIgnore]
    public Guid UserId { get; set; }
}