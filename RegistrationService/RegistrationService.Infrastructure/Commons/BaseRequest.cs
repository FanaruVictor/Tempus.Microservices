using MediatR;
using Newtonsoft.Json;

namespace RegistrationService.Infrastructure.Commons;

public class BaseRequest<TResponse> : IRequest<TResponse>
{
    [JsonIgnore] public Guid UserId { get; set; }
}