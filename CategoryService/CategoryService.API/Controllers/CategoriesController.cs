using CategoryService.Core.Models.Category;
using CategoryService.Infrastructure.Commands.UserCategory.Create;
using CategoryService.Infrastructure.Commands.UserCategory.Delete;
using CategoryService.Infrastructure.Commands.UserCategory.Update;
using CategoryService.Infrastructure.Queries.Categories.GetAll;
using CategoryService.Infrastructure.Queries.Categories.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CategoryService.API.Controllers;

/// <summary>
///     CategoryController is responsible with requests designed for categories
/// </summary>
public class CategoriesController : BaseController
{
    /// <summary>
    ///     constructor
    /// </summary>
    /// <param name="mediator"></param>
    public CategoriesController(IMediator mediator) : base(mediator) { }

    /// <summary>
    ///     Get all categories. If UserId is specified. This action will return all categories created by the specified user,
    ///     otherwise it will return all categories from database
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<BaseCategory>>> GetAll([FromQuery] Guid groupId)
    {
        return HandleResponse(await _mediator.Send(new GetAllCategoriesQuery
        {
            GroupId = groupId
        }));
    }

    /// <summary>
    ///     For a specified Id a category will be returned if it exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<BaseCategory>> GetById([FromRoute] Guid id, [FromQuery] Guid groupId)
    {
        return HandleResponse(await _mediator.Send(new GetCategoryByIdQuery { Id = id, GroupId = groupId }));
    }

    /// <summary>
    ///     Create a category and saves it into database
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<BaseCategory>> Create([FromBody] CreateCategoryCommand command, [FromQuery] Guid groupId)
    {
        command.GroupId = groupId;
        return HandleResponse(await _mediator.Send(command));
    }

    /// <summary>
    ///     Updates a category proprieties
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<BaseCategory>> Update([FromBody] UpdateCategoryCommand command, [FromQuery] Guid groupId)
    {
        command.GroupId = groupId;
        return HandleResponse(await _mediator.Send(command));
    }

    /// <summary>
    ///     For a specified Id a category will be deleted from database if it exists
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> Delete([FromRoute] Guid id, [FromQuery] Guid groupId)
    {
        return HandleResponse(await _mediator.Send(new DeleteCategoryCommand
        {
            Id = id,
            GroupId = groupId
        }));
    }
}