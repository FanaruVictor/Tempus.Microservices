using MediatR;
using Microsoft.AspNetCore.Http;
using UserService.Infrastructure.Commons;

namespace UserService.Infrastructure;

public class MediatrRequestContextBehaviour<TRequest, TResponse>(IHttpContextAccessor contextAccessor) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {

        if (_contextAccessor.HttpContext.Request.Path.Value.Equals("/api/users", StringComparison.InvariantCultureIgnoreCase) && _contextAccessor.HttpContext.Request.Method == HttpMethod.Post.ToString()
            || _contextAccessor.HttpContext.Request.Path.Value.StartsWith("/api/users/loginCredentials"))
        {
            return await next();
        }

        if ((_contextAccessor.HttpContext?.Request.Headers.TryGetValue("UserId", out var userIdHeader) ?? false)
            && Guid.TryParse(userIdHeader.ToString(), out var userId))
        {
            request.UserId = userId;

            return await next();
        }

        throw new UnauthorizedAccessException("User doesn't have the necessary claims");
    }
}