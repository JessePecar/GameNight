using Microsoft.AspNetCore.Mvc;

namespace GameNight.API.Controllers
{
    [ApiController]
    [Route("api/ChooseOne")]
    public class ChooseOneController : Controller
    {

        [HttpGet]
        [Route("Prompt")]
        public async Task<IActionResult> Prompt()
        {
            var client = new HttpClient();

            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://random-word-api.herokuapp.com/word?number=5&swear=0"));

            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            return StatusCode(500, "Unable to get prompt :(");
        }
    }
}
