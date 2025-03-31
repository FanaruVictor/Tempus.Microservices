using APIGateway.IServices;
using APIGateway.Models.Registrations;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    public class RegistrationsController(IRegistrationService registrationService, IHttpContextAccessor contextAccessor) : BaseController(contextAccessor)
    {
        private readonly IRegistrationService registrationService = registrationService;
        [HttpGet]
        public async Task<ActionResult<List<RegistrationDetails>>> GetAll([FromQuery] Guid groupId)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var response = await this.registrationService.GetAll(userId, groupId);

            if (response == null || !response.Any())
            {
                return NoContent();
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistrationDetails>> GetById([FromRoute] Guid id, [FromQuery] Guid groupId)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var response = await this.registrationService.GetById(userId, id, groupId);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<RegistrationDetails>> Create([FromBody] NewRegistration newRegistration, [FromQuery] Guid groupId)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await this.registrationService.Create(userId, newRegistration, groupId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult<RegistrationDetails>> Update([FromBody] RegistrationInfo registrationInfo, [FromQuery] Guid groupId)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result = await this.registrationService.Update(userId, registrationInfo, groupId);

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

            var result = await this.registrationService.Delete(userId, id, groupId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("lastUpdated")]
        public async Task<ActionResult<RegistrationDetails>> GetLastUpdated([FromQuery] Guid groupId)
        {
            var userId = GetUserIdFromRequest();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var response = await this.registrationService.GetLastUpdated(userId, groupId);

            if (response == null)
            {
                return NoContent();
            }

            return Ok(response);
        }
    }
}
