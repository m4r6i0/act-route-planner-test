namespace RoutePlanner.API.Models
{
    public class TravelRoute
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int Cost { get; set; }

        public TravelRoute(string origin, string destination, int cost)
        {
            Origin = origin;
            Destination = destination;
            Cost = cost;
        }
    }
}
