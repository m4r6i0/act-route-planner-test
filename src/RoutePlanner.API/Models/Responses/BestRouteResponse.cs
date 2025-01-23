namespace RoutePlanner.API.Models.Responses
{
    public class BestRouteResponse
    {
        public string Route { get; set; } = string.Empty;
        public int Cost { get; set; } = 0;

        public BestRouteResponse(string route, int cost)
        {
            Route = route;
            Cost = cost;
        }
    }
}
