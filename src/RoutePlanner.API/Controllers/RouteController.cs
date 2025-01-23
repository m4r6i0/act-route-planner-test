using Microsoft.AspNetCore.Mvc;
using RoutePlanner.API.Models.Requests;
using RoutePlanner.API.Models.Responses;
using RoutePlanner.Domain.Entities;
using RoutePlanner.API.Services;
using System.Threading.Tasks;

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

        /// <summary>
        /// Registra uma nova rota.
        /// </summary>
        /// <param name="request">Dados da rota a ser registrada.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterRoute([FromBody] TravelRouteRequest request)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors.Select(e => e.ErrorMessage);
                    Console.WriteLine($"Validation Error in {key}: {string.Join(", ", errors)}");
                }

                return BadRequest(new
                {
                    Message = "Validation failed.",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            await _service.AddRouteAsync(new TravelRoute(request.Origin, request.Destination, request.Cost));

            return Ok(new { Message = "Route registered successfully!" });
        }


        /// <summary>
        /// Obtém a melhor rota entre dois pontos.
        /// </summary>
        /// <param name="origin">Local de partida.</param>
        /// <param name="destination">Local de chegada.</param>
        /// <returns>Melhor rota e custo ou mensagem de erro.</returns>
        [HttpGet("best")]
        public async Task<IActionResult> GetBestRoute(string origin, string destination)
        {
            var response = await _service.GetBestRouteAsync(origin, destination);

            if (response.Route == "No route available")
            {
                return NotFound(new { Message = response.Route });
            }

            return Ok(response);
        }
    }
}
