namespace RoutePlanner.Domain.Entities
{
    public class TravelRoute
    {
        public int Id { get; set; } // Chave primária gerada automaticamente
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
