using MediatR;
using Newtonsoft.Json;

namespace GroupService.Infrastructure.Commons;

public class BaseRequest<TResponse> : IRequest<TResponse>
{
    [JsonIgnore] public Guid UserId { get; set; }
}