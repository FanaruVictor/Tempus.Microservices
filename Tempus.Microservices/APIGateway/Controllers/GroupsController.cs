using APIGateway.IServices;
using APIGateway.Models.Group;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    public class GroupsController(IHttpContextAccessor contextAccessor, IGroupService groupService) : BaseController(contextAccessor)
    {
        private readonly IGroupService groupService = groupService;

        [HttpPost]
        public async Task<ActionResult<bool>> Add([FromForm] NewGroup newGroup)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await this.groupService.Create(userId, newGroup);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<GroupOverview>>> GetAll()
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var response = await this.groupService.GetAll(userId);

            if (response == null || !response.Any())
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDetails>> GetById([FromRoute] Guid id)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var response = await this.groupService.GetById(userId, id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<GroupOverview>> Update([FromForm] GroupInfo groupInfo)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await this.groupService.Update(userId, groupInfo);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Guid>> Delete([FromRoute] Guid id)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await this.groupService.Delete(userId, id);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
