using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tempus.Shared.Commons;

namespace UserService.Infrastructure;

public class MediatrRequestContextBehaviour<TRequest, TResponse>(IHttpContextAccessor contextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>
{
    [DebuggerStepThrough]
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if((contextAccessor.HttpContext?.Request.Headers.TryGetValue("UserId", out var userIdHeader) ?? false)
           && Guid.TryParse(userIdHeader.ToString(), out var userId))
        {
            request.UserId = userId;

            return await next();
        }

        request.UserId = Guid.Parse("285cc2f3-d6c1-40fd-8d21-59e1cd578a52");

        return await next();

        throw new UnauthorizedAccessException("User doesn't have the necessary claims");
    }
}