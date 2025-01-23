using RoutePlanner.API.Models.Responses;
using RoutePlanner.Domain.Entities;
using RoutePlanner.Domain.Interfaces;

namespace RoutePlanner.API.Services
{
    public class RouteService
    {
        private readonly IRepository<TravelRoute> _repository;

        public RouteService(IRepository<TravelRoute> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Adiciona uma nova rota ao repositório, validando duplicatas.
        /// </summary>
        /// <param name="route">A rota a ser adicionada.</param>
        /// <exception cref="ArgumentException">Lançada se a rota já existir.</exception>
        public async Task AddRouteAsync(TravelRoute route)
        {
            if (string.IsNullOrEmpty(route.Origin) || string.IsNullOrEmpty(route.Destination))
            {
                throw new ArgumentException("Origin and Destination cannot be null or empty.");
            }

            if (route.Cost <= 0)
            {
                throw new ArgumentException("Cost must be greater than zero.");
            }

            var existingRoutes = await _repository.GetAllAsync();
            if (existingRoutes.Any(r => r.Origin == route.Origin && r.Destination == route.Destination))
            {
                throw new ArgumentException("This route already exists.");
            }

            await _repository.AddAsync(route);
        }

        /// <summary>
        /// Encontra a melhor rota (de menor custo) entre a origem e o destino.
        /// </summary>
        /// <param name="origin">Local de partida.</param>
        /// <param name="destination">Local de chegada.</param>
        /// <returns>Um objeto contendo a melhor rota e seu custo.</returns>
        public async Task<BestRouteResponse> GetBestRouteAsync(string origin, string destination)
        {
            // Obtém todas as rotas disponíveis do repositório
            var routes = await _repository.GetAllAsync();

            // Verifica se há rotas registradas
            if (routes == null || routes.Count == 0)
            {
                return new BestRouteResponse("No route available", 0);
            }

            // Encontra todos os caminhos possíveis entre origem e destino
            var allPaths = FindAllTravelPaths(routes, origin, destination);

            // Verifica se há caminhos possíveis
            if (allPaths == null || allPaths.Count == 0)
            {
                return new BestRouteResponse("No route available", 0);
            }

            // Seleciona o caminho com o menor custo
            var bestPath = allPaths.OrderBy(p => p.Cost).FirstOrDefault();

            return bestPath != null
                ? new BestRouteResponse(string.Join(" - ", bestPath.Nodes), bestPath.Cost)
                : new BestRouteResponse("No route available", 0);
        }

        /// <summary>
        /// Verifica se uma rota já existe no repositório.
        /// </summary>
        /// <param name="route">A rota a ser verificada.</param>
        /// <returns>True se a rota já existir; caso contrário, False.</returns>
        private async Task<bool> RouteExistsAsync(TravelRoute route)
        {
            var routes = await _repository.GetAllAsync();
            return routes.Any(r => r.Origin == route.Origin && r.Destination == route.Destination);
        }

        /// <summary>
        /// Encontra todos os caminhos possíveis entre dois pontos.
        /// </summary>
        /// <param name="routes">Lista de rotas disponíveis.</param>
        /// <param name="start">Ponto de partida.</param>
        /// <param name="end">Ponto de chegada.</param>
        /// <returns>Uma lista de caminhos com seus respectivos custos.</returns>
        private List<TravelPath> FindAllTravelPaths(List<TravelRoute> routes, string start, string end)
        {
            var paths = new List<TravelPath>();
            ExplorePathsRecursively(routes, start, end, new List<string>(), 0, paths);
            return paths;
        }

        /// <summary>
        /// Explora os caminhos recursivamente usando busca em profundidade.
        /// </summary>
        /// <param name="routes">Lista de rotas disponíveis.</param>
        /// <param name="current">Local atual na busca.</param>
        /// <param name="end">Destino desejado.</param>
        /// <param name="visited">Caminho visitado até o momento.</param>
        /// <param name="currentCost">Custo acumulado no caminho atual.</param>
        /// <param name="paths">Lista para armazenar os caminhos encontrados.</param>
        private void ExplorePathsRecursively(
            List<TravelRoute> routes,
            string current,
            string end,
            List<string> visited,
            int currentCost,
            List<TravelPath> paths)
        {
            visited.Add(current);

            // Registra o caminho se o destino for alcançado
            if (current == end)
            {
                paths.Add(new TravelPath(new List<string>(visited), currentCost));
            }
            else
            {
                // Explora todos os destinos conectados
                foreach (var route in routes.Where(r => r.Origin == current && !visited.Contains(r.Destination)))
                {
                    ExplorePathsRecursively(routes, route.Destination, end, visited, currentCost + route.Cost, paths);
                }
            }

            // Backtracking: Remove o nó atual antes de retornar
            visited.RemoveAt(visited.Count - 1);
        }
    }
}
