using MediatR;
using Microsoft.AspNetCore.Mvc;
using RegistrationService.Core.Models.Registrations;
using RegistrationService.Infrastructure.Commands.Registrations.Create;
using RegistrationService.Infrastructure.Commands.Registrations.Delete;
using RegistrationService.Infrastructure.Commands.Registrations.Update;
using RegistrationService.Infrastructure.Queries.Registrations.GetAll;
using RegistrationService.Infrastructure.Queries.Registrations.GetById;
using RegistrationService.Infrastructure.Queries.Registrations.LastUpdated;

namespace RegistrationService.API.Controllers;

/// <summary>
///     RegistrationsConstructor is responsible with requests designed for registrations
/// </summary>
public class RegistrationsController : BaseController
{
    /// <summary>
    ///     constructor
    /// </summary>
    /// <param name="mediator"></param>
    public RegistrationsController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    ///     Get all registration. If a CategoryId is specified this action will return all registrations created for the
    ///     specified category,
    ///     otherwise it will return all registrations from database
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<RegistrationOverview>>> GetAll([FromQuery] GetAllRegistrationsQuery query)
    {
        return HandleResponse(await _mediator.Send(query));
    }

    /// <summary>
    ///     For a specified Id a registration will be returned if it exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RegistrationDetails>> GetById([FromRoute] Guid id, [FromQuery] Guid? groupId)
    {
        return HandleResponse(await _mediator.Send(new GetRegistrationByIdQuery { Id = id, GroupId = groupId }));
    }

    /// <summary>
    ///     Create a registration and saves it into database
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<RegistrationOverview>> Create([FromBody] CreateRegistrationCommand command)
    {
        return HandleResponse(await _mediator.Send(command));
    }

    /// <summary>
    ///     Updates a registration proprieties
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<RegistrationDetails>> Update([FromBody] UpdateRegistrationCommand command)
    {
        return HandleResponse(await _mediator.Send(command));
    }

    /// <summary>
    ///     For a specified Id a registration will be deleted from database if it exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> Delete([FromRoute] Guid id, [FromQuery] Guid? groupId)

    {
        return HandleResponse(await _mediator.Send(new DeleteRegistrationCommand
        {
            Id = id,
            GroupId = groupId
        }));
    }

    /// <summary>
    ///     Get last registration updated
    /// </summary>
    /// <returns></returns>
    [HttpGet("lastUpdated")]
    public async Task<ActionResult<BaseRegistration>> GetLastUpdated()
    {
        return HandleResponse(await _mediator.Send(new GetLastUpdatedRegsitrationQuery()));
    }
}