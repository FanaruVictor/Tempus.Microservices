using CategoryService.Infrastructure.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CategoryService.Infrastructure;

public class MediatrRequestContextBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public MediatrRequestContextBehaviour(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if ((_contextAccessor.HttpContext?.Request.Headers.TryGetValue("UserId", out var userIdHeader) ?? false)
            && Guid.TryParse(userIdHeader.ToString(), out var userId))
        {
            request.UserId = userId;

            return await next();
        }

        throw new UnauthorizedAccessException("User doesn't have the necessary claims");

    }
}