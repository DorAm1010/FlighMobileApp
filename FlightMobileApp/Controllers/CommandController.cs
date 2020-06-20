using FlightMobileApp.Client;
using FlightMobileApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlightMobileApp.Controllers
{
    [Route("api/command")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private ITcpClient _client;

        public CommandController(ITcpClient client)
        {
            _client = client;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Command command)
        {
            Result result = await _client.Execute(command);

            if (result == Result.Ok)
                return Ok(command);

            return BadRequest(command);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var image = await _client.GetScreenshot();
            return File(image, "image/png");
        }
    }


}