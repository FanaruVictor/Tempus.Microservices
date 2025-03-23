using APIGateway.Controllers;
using APIGateway.IServices;
using APIGateway.Models.Category;
using Microsoft.AspNetCore.Mvc;

namespace Tempus.API.Controllers;

public class CategoriesController(IHttpContextAccessor contextAccessor, ICategoryService categoryService) : BaseController(contextAccessor)
{
    private readonly ICategoryService categoryService = categoryService;

    [HttpGet]
    public async Task<ActionResult<List<BaseCategory>>> GetAll([FromQuery] Guid groupId)
    {
        var userId = GetUserIdFromRequest();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var response = await this.categoryService.GetAll(userId, groupId);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseCategory>> GetById([FromRoute] Guid id, [FromQuery] Guid groupId)
    {
        var userId = GetUserIdFromRequest();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var response = await this.categoryService.GetById(userId, id, groupId);

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<BaseCategory>> Create([FromBody] NewCategory newCategory)
    {
        var userId = GetUserIdFromRequest();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await this.categoryService.Create(userId, newCategory);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);

    }

    [HttpPut]
    public async Task<ActionResult<BaseCategory>> Update([FromBody] CategoryInfo categoryInfo, [FromQuery] Guid groupId)
    {
        var userId = GetUserIdFromRequest();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await this.categoryService.Update(userId, categoryInfo, groupId);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> Delete([FromRoute] Guid id, [FromQuery] Guid groupId)
    {
        var userId = GetUserIdFromRequest();

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await this.categoryService.Delete(userId, id, groupId);

        if (result == Guid.Empty)
        {
            return BadRequest();
        }

        return Ok(result);
    }
}