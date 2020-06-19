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
            if (await _client.Execute(command)) return Ok(command);

            return BadRequest(command);
        }
    }
}