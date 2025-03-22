using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers
{
    [ApiController, Route("api/[controller]"), Authorize]
    public class RegistrationsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<int>> GetAll()
        {
            return Ok(new List<int> { 1, 3, 4 });
        }
    }
}
