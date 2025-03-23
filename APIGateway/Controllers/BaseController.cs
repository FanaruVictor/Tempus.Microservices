using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APIGateway.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class BaseController(IHttpContextAccessor contextAccessor) : ControllerBase
    {
        private readonly IHttpContextAccessor contextAccessor = contextAccessor;

        protected Guid GetUserIdFromRequest()
        {
            var userIdClaim =
               this.contextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException("User doesn't have the necessary claims");

            if (Guid.TryParse(userIdClaim?.Value, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
    }
}
