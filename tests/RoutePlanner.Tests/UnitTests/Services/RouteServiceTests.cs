using RoutePlanner.API.Repositories;
using RoutePlanner.API.Services;
using RoutePlanner.Domain.Entities;
using RoutePlanner.Tests.Helpers;

namespace RoutePlanner.Tests.UnitTests.Services
{
    public class RouteServiceTests
    {
        [Fact]
        public async Task GetBestRoute_ShouldReturn_CheapestRoute()
        {

            using var context = InMemoryDbContextFactory.CreateDbContext();
            context.Database.EnsureDeleted();
            

            var repository = new RouteRepository(context);
            var service = new RouteService(repository);

            await repository.AddAsync(new TravelRoute("GRU", "BRC", 10));
            await repository.AddAsync(new TravelRoute("BRC", "SCL", 5));
            await repository.AddAsync(new TravelRoute("GRU", "CDG", 75));
            await repository.AddAsync(new TravelRoute("GRU", "SCL", 20));
            await repository.AddAsync(new TravelRoute("SCL", "ORL", 20));
            await repository.AddAsync(new TravelRoute("ORL", "CDG", 5));

            // Act
            var result = await service.GetBestRouteAsync("GRU", "CDG");

            // Assert
            Assert.Equal("GRU - BRC - SCL - ORL - CDG", result.Route);
            Assert.Equal(40, result.Cost);

        }

        [Fact]
        public async Task GetBestRoute_NoRouteExists_ShouldReturn_EmptyRoute()
        {
            // Arrange
            using var context = InMemoryDbContextFactory.CreateDbContext();
            context.Database.EnsureDeleted();
            var repository = new RouteRepository(context);
            var service = new RouteService(repository);

            // Act
            var result = await service.GetBestRouteAsync("GRU", "CDG");

            // Assert
            Assert.Equal("No route available", result.Route);
            Assert.Equal(0, result.Cost);
        }
    }
}
