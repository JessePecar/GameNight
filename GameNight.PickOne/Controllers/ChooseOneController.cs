using Microsoft.AspNetCore.Mvc;

namespace GameNight.API.Controllers
{
    [ApiController]
    [Route("api/ChooseOne")]
    public class ChooseOneController : Controller
    {

        [HttpGet]
        [Route("Prompt")]
        public IActionResult Prompt()
        {
            return Ok("Pickles");
        }
    }
}
