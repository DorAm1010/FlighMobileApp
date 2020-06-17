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
        public /*async*/ IActionResult Post([FromBody] Command command)
        {
            // try set value of aileron
            if(!_client.SetProperty("/flight/aileron",command.Aileron))
                return BadRequest(SetFailed("aileron", command.Aileron));
            
            // try set value of elevator
            if (!_client.SetProperty("/flight/elevator", command.Elevator)) 
                return BadRequest(SetFailed("elevator", command.Elevator));

            // try set value of rudder
            if (!_client.SetProperty("/flight/rudder", command.Rudder)) 
                return BadRequest(SetFailed("rudder", command.Rudder));

            // try set value of throttle
            if (!_client.SetProperty("/engines/current-engine/throttle", command.Throttle)) 
                return BadRequest(SetFailed("throttle", command.Throttle));

            return Ok();
        }

        private string SetFailed(string propertyName, double value)
        {
            return "Failed Setting " + propertyName + "With Value Of " + value.ToString() + "\n";
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var image = await _client.GetScreenshot();
            return File(image, "image/png");
        }
    }


}