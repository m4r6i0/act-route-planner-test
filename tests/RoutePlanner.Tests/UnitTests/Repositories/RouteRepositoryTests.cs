using RoutePlanner.API.Repositories;
using RoutePlanner.Domain.Entities;
using RoutePlanner.Tests.Helpers;


namespace RoutePlanner.Tests.UnitTests.Repositories
{
    public class RouteRepositoryTests
    {
        [Fact]
        public async Task AddRoute_ShouldPersistRouteInDatabase()
        {
            // Arrange
            using var context = InMemoryDbContextFactory.CreateDbContext();
            context.Database.EnsureDeleted();
            var repository = new RouteRepository(context);
            var route = new TravelRoute("GRU", "BRC", 10);

            // Act
            await repository.AddAsync(route);

            // Assert
            var routes = await repository.GetAllAsync();
            Assert.Single(routes);
            Assert.Equal("GRU", routes[0].Origin);
            Assert.Equal("BRC", routes[0].Destination);
            Assert.Equal(10, routes[0].Cost);
        }

        [Fact]
        public async Task GetAllRoutes_ShouldReturnAllPersistedRoutes()
        {
            // Arrange
            using var context = InMemoryDbContextFactory.CreateDbContext();
            context.Database.EnsureDeleted();
            var repository = new RouteRepository(context);

            await repository.AddAsync(new TravelRoute("GRU", "BRC", 10));
            await repository.AddAsync(new TravelRoute("BRC", "SCL", 5));

            // Act
            var routes = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, routes.Count);
            Assert.Contains(routes, r => r.Origin == "GRU" && r.Destination == "BRC" && r.Cost == 10);
            Assert.Contains(routes, r => r.Origin == "BRC" && r.Destination == "SCL" && r.Cost == 5);
        }
    }
}
