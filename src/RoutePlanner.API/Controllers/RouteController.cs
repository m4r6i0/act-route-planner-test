using Microsoft.AspNetCore.Mvc;
using RoutePlanner.API.Models;
using RoutePlanner.API.Services;

namespace RoutePlanner.API.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RouteController : ControllerBase
    {
        private readonly RouteService _service;

        public RouteController(RouteService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public IActionResult RegisterRoute([FromBody] TravelRoute route)
        {
            _service.AddRoute(route);
            return Ok("Route registered successfully!");
        }

        [HttpGet("best")]
        public IActionResult GetBestRoute(string origin, string destination)
        {
            var result = _service.GetBestRoute(origin, destination);
            return Ok(new { Route = result.route, Cost = result.cost });
        }
    }
}
