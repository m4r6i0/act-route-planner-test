using System.ComponentModel.DataAnnotations;

namespace RoutePlanner.API.Models.Requests
{
    public class TravelRouteRequest
    {
        [Required(ErrorMessage = "Origin is required.")]
        public string? Origin { get; set; }

        [Required(ErrorMessage = "Destination is required.")]
        public string? Destination { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Cost must be greater than 0.")]
        public int Cost { get; set; }
    }
}
