using Microsoft.EntityFrameworkCore;
using RoutePlanner.API.Data;
using RoutePlanner.Domain.Entities;
using RoutePlanner.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoutePlanner.API.Repositories
{
    public class RouteRepository : IRepository<TravelRoute>
    {
        private readonly RoutePlannerDbContext _context;

        public RouteRepository(RoutePlannerDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona uma nova rota ao banco de dados.
        /// </summary>
        /// <param name="entity">A rota a ser adicionada.</param>
        public async Task AddAsync(TravelRoute entity)
        {
            if (await _context.TravelRoutes.AnyAsync(r => r.Origin == entity.Origin && r.Destination == entity.Destination))
            {
                throw new InvalidOperationException("Route already exists.");
            }

            _context.TravelRoutes.Add(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retorna todas as rotas armazenadas no banco de dados.
        /// </summary>
        /// <returns>Uma lista de rotas.</returns>
        public async Task<List<TravelRoute>> GetAllAsync()
        {
            return await _context.TravelRoutes.ToListAsync();
        }

        /// <summary>
        /// Retorna uma rota específica com base na origem e no destino.
        /// </summary>
        /// <param name="origin">A origem da rota.</param>
        /// <param name="destination">O destino da rota.</param>
        /// <returns>A rota encontrada ou uma rota padrão, se não existir.</returns>
        public async Task<TravelRoute> GetByOriginAndDestinationAsync(string origin, string destination)
        {
            return await _context.TravelRoutes
                .FirstOrDefaultAsync(r => r.Origin == origin && r.Destination == destination)
                ?? new TravelRoute(origin, destination, 0); // Retorna um objeto com valores padrão.
        }

    }
}
