using RoutePlanner.API.Models;
using RoutePlanner.API.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace RoutePlanner.API.Services
{
    public class RouteService
    {
        private readonly RouteRepository _repository;

        public RouteService(RouteRepository repository)
        {
            _repository = repository;
        }

        public void AddRoute(TravelRoute route)
        {
            _repository.AddRoute(route);
        }

        public (string route, int cost) GetBestRoute(string origin, string destination)
        {
            var routes = _repository.GetAllRoutes();
            var allPaths = FindPaths(routes, origin, destination);
            var bestPath = allPaths.OrderBy(p => p.Cost).FirstOrDefault();
            return bestPath != null ? (string.Join(" - ", bestPath.Nodes), bestPath.Cost) : ("", 0);
        }

        private List<Path> FindPaths(List<TravelRoute> routes, string start, string end)
        {
            var paths = new List<Path>();
            DFS(routes, start, end, new List<string>(), 0, paths);
            return paths;
        }

        private void DFS(List<TravelRoute> routes, string current, string end, List<string> visited, int cost, List<Path> paths)
        {
            visited.Add(current);

            if (current == end)
            {
                paths.Add(new Path(new List<string>(visited), cost));
                visited.RemoveAt(visited.Count - 1);
                return;
            }

            foreach (var route in routes.Where(r => r.Origin == current && !visited.Contains(r.Destination)))
            {
                DFS(routes, route.Destination, end, visited, cost + route.Cost, paths);
            }

            visited.RemoveAt(visited.Count - 1);
        }
    }

    public record Path(List<string> Nodes, int Cost);
}
