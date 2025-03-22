using MediatR;
using Microsoft.AspNetCore.Mvc;
using UserService.Infrastructure.Commands.Users.ChangeTheme;
using UserService.Infrastructure.Commands.Users.Create;
using UserService.Infrastructure.Commands.Users.Delete;
using UserService.Infrastructure.Commands.Users.Update;
using UserService.Infrastructure.Models;
using UserService.Infrastructure.Queries.Users.GetAll;
using UserService.Infrastructure.Queries.Users.GetById;
using UserService.Infrastructure.Queries.Users.GetEmails;
using UserService.Infrastructure.Queries.Users.GetTheme;
using UserService.Infrastructure.Queries.Users.GetUserByEmail;

namespace UserService.API.Controllers
{
    /// <summary>
    ///     constructor
    /// </summary>
    /// <param name="mediator"></param>
    public class UsersController(IMediator mediator) : BaseController(mediator)
    {

        /// <summary>
        ///     Get all users from database
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<List<UserDetails>>> GetAll()
        {
            return HandleResponse(await _mediator.Send(new GetAllUsersQuery()));
        }

        /// <summary>
        ///     For a specified Id a user will be returned if it exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDetails>> GetById([FromRoute] Guid id)
        {
            return HandleResponse(await _mediator.Send(new GetUserByIdQuery
            {
                Id = id
            }));
        }

        /// <summary>
        ///     Update an user proprieties
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<UserDetails>> Update([FromForm] UpdateUserCommand command)
        {
            return HandleResponse(await _mediator.Send(command));
        }

        /// <summary>
        ///     For a specified Id a user will be deleted from database  if it exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<Guid>> Delete()
        {
            return HandleResponse(await _mediator.Send(new DeleteUserCommand()));
        }

        [HttpPut("changeTheme")]
        public async Task<ActionResult<UserDetails>> ChangeTheme([FromBody] ChangeThemeCommand command)
        {
            return HandleResponse(await _mediator.Send(command));
        }

        [HttpGet("theme")]
        public async Task<ActionResult<bool>> GetTheme()
        {
            return HandleResponse(await _mediator.Send(new GetThemeQuery()));
        }

        [HttpGet("emails")]
        public async Task<ActionResult<List<UserEmail>>> GetEmails()
        {
            return HandleResponse(await _mediator.Send(new GetEmailsQuery()));
        }

        [HttpPost]
        public async Task<ActionResult<UserDetails>> Create([FromBody] CreateUserCommand command)
        {
            return HandleResponse(await _mediator.Send(command));
        }

        [HttpGet("loginCredentials/{email}")]
        public async Task<ActionResult<LoginCredentials>> GetLoginCredentials([FromRoute] string email)
        {
            return HandleResponse(await _mediator.Send(new GetLoginCredentialsHandler { Email = email }));
        }
    }
}
