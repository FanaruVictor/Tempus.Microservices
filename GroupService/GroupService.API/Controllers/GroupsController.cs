using GroupService.Infrastructure.Commands.Groups.Create;
using GroupService.Infrastructure.Commands.Groups.Delete;
using GroupService.Infrastructure.Commands.Groups.Update;
using GroupService.Infrastructure.Models;
using GroupService.Infrastructure.Queries.Groups.GetAllGroupsQuery;
using GroupService.Infrastructure.Queries.Groups.GetGroupByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroupService.API.Controllers;

public class GroupsController : BaseController
{
    public GroupsController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    public async Task<ActionResult<bool>> Add([FromForm] CreateGroupCommand command)
    {
        return HandleResponse(await _mediator.Send(command));
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupOverview>>> GetAll()
    {
        return HandleResponse(await _mediator.Send(new GetAllGroupsQuery()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> Delete([FromRoute] Guid id)
    {
        return HandleResponse(await _mediator.Send(new DeleteGroupCommand() { Id = id }));
    }

    [HttpGet("{id:Guid}")]
    public async Task<ActionResult<GroupDetails>> GetById([FromRoute] Guid id)
    {
        return HandleResponse(await _mediator.Send(new GetGroupByIdQuery { Id = id }));
    }

    [HttpPut]
    public async Task<ActionResult<GroupOverview>> Update([FromForm] UpdateGroupCommand command)
    {
        return HandleResponse(await _mediator.Send(command));
    }
}