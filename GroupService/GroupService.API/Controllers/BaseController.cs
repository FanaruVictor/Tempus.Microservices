using GroupService.Core.Commons;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CustomStatusCodes = GroupService.Core.Commons.StatusCodes;

namespace GroupService.API.Controllers;

/// <summary>
///     Base controller, every controller will inherited this one
/// </summary>
[ApiController, Route("api/[controller]")]
public class BaseController : ControllerBase
{
    /// <summary>
    /// </summary>
    protected readonly IMediator _mediator;

    /// <summary>
    ///     constructor
    /// </summary>
    /// <param name="mediator"></param>
    public BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }

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
            CustomStatusCodes.Unauthorized => Unauthorized(response)
        };
    }
}