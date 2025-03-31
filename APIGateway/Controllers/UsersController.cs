using APIGateway.IServices;
using APIGateway.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class UsersController(IUserService userService, IHttpContextAccessor contextAccessor) : BaseController(contextAccessor)
{
    private readonly IUserService userService = userService;

    [HttpGet]
    public async Task<ActionResult<List<UserDetails>>> GetAll()
    {
        var id = GetUserIdFromRequest();

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var response = await this.userService.GetAll(id);

        if (response == null || !response.Any())
        {
            return NoContent();
        }

        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDetails>> GetById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
        {
            id = GetUserIdFromRequest();
        }

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var response = await this.userService.GetById(id);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult<UserDetails>> Update([FromForm] UserInfo user)
    {
        var id = GetUserIdFromRequest();

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await this.userService.Update(id, user);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpDelete]
    public async Task<ActionResult<Guid>> Delete()
    {
        var id = GetUserIdFromRequest();

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await this.userService.Delete(id);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);

    }

    [HttpPut("changeTheme")]
    public async Task<ActionResult<UserDetails>> ChangeTheme([FromBody] bool isDarkTheme)
    {
        var id = GetUserIdFromRequest();

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await this.userService.ChangeTheme(isDarkTheme, id);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpGet("theme")]
    public async Task<ActionResult<bool>> GetTheme()
    {
        var id = GetUserIdFromRequest();

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var response = await this.userService.GetTheme(id);

        if (response == null)
        {
            return BadRequest();
        }

        return Ok(response);
    }

    [HttpGet("emails")]
    public async Task<ActionResult<List<UserEmail>>> GetEmails()
    {
        var id = GetUserIdFromRequest();

        if (id == Guid.Empty)
        {
            return Unauthorized();
        }

        var response = await this.userService.GetEmails(id);

        return Ok(response);
    }
}