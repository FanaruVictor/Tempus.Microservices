using MediatR;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Security.Claims;
using UserService.Infrastructure.Commons;

namespace UserService.Infrastructure;

public class MediatrRequestContextBehaviour<TRequest, TResponse>(IHttpContextAccessor contextAccessor) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseRequest<TResponse>
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    [DebuggerStepThrough]
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            _contextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User doesn't have the necessary claims");

        if (Guid.TryParse(userIdClaim?.Value, out var userId))
        {
            request.UserId = userId;
        }

        return await next();
    }
}