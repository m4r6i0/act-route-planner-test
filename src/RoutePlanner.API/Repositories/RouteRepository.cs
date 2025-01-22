using RoutePlanner.API.Models;

namespace RoutePlanner.API.Repositories
{
    public class RouteRepository
    {
        private readonly string _filePath;

        public RouteRepository(string filePath)
        {
            _filePath = filePath;
        }

        public void AddRoute(TravelRoute route)
        {
            var routeLine = $"{route.Origin},{route.Destination},{route.Cost}";
            File.AppendAllLines(_filePath, new[] { routeLine });
        }

        public List<TravelRoute> GetAllRoutes()
        {
            if (!File.Exists(_filePath))
                return new List<TravelRoute>();

            return File.ReadAllLines(_filePath)
                .Select(line =>
                {
                    var parts = line.Split(',');
                    return new TravelRoute(parts[0], parts[1], int.Parse(parts[2]));
                })
                .ToList();
        }
    }
}
