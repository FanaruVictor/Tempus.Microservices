using APIGatewat.IServices;
using APIGatewat.Models;
using APIGateway.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers;

[AllowAnonymous, ApiController, Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    public async Task<ActionResult> Register([FromBody] NewUser newUser)
    {
        var result = await _authService.Register(newUser);

        if (result != null)
        {
            return Ok(result);
        }

        return BadRequest();
    }

    [HttpPost("login")]
    public async Task<ActionResult<bool>> Login([FromBody] LoginCredentials credentials)
    {
        var result = await _authService.Login(credentials);

        if (result != null)
        {
            return Ok(result);
        }

        return BadRequest();
    }
}

