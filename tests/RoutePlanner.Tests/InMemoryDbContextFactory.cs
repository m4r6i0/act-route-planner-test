using Microsoft.EntityFrameworkCore;
using RoutePlanner.API.Data;

namespace RoutePlanner.Tests.Helpers
{
    public static class InMemoryDbContextFactory
    {
        /// <summary>
        /// Cria um contexto do banco de dados em memória para os testes.
        /// </summary>
        /// <returns>Instância do RoutePlannerDbContext configurada.</returns>
        public static RoutePlannerDbContext CreateDbContext(string databaseName = "TestDatabase")
        {
            var options = new DbContextOptionsBuilder<RoutePlannerDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            var context = new RoutePlannerDbContext(options);

            // Garante que o banco de dados está limpo antes de cada teste
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}
