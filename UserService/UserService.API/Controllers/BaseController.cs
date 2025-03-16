using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Infrastructure.Commons;
using CustomStatusCodes = UserService.Infrastructure.Commons.StatusCodes;

namespace UserService.API.Controllers;

/// <summary>
///     Base controller, every controller will inherited this one
/// </summary>
/// <remarks>
///     constructor
/// </remarks>
/// <param name="mediator"></param>
[Authorize, ApiController, Route("api/[controller]")]
public class BaseController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// </summary>
    protected readonly IMediator _mediator = mediator;

    /// <summary>
    ///     Handles the response from the mediatr
    /// </summary>
    /// <param name="response"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected ActionResult<T> HandleResponse<T>(BaseResponse<T> response)
    {
        return response.StatusCode switch
        {
            CustomStatusCodes.Ok => Ok(response),
            CustomStatusCodes.Created => Created(new Uri(""), response),
            CustomStatusCodes.NotFound => NotFound(),
            CustomStatusCodes.BadRequest => BadRequest(response),
            CustomStatusCodes.Unauthorized => Unauthorized(response),
            CustomStatusCodes.Forbidden => Forbid()
        };
    }
}
