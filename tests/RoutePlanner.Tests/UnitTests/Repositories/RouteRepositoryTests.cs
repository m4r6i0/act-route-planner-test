using RoutePlanner.API.Models;
using RoutePlanner.API.Repositories;
using System.IO;
using Xunit;

namespace RoutePlanner.Tests.UnitTests.Repositories
{
    public class RouteRepositoryTests
    {
        [Fact]
        public void AddRoute_ShouldPersistRouteInFile()
        {
            // Arrange
            var filePath = "test_routes.txt";
            if (File.Exists(filePath)) File.Delete(filePath);

            var repository = new RouteRepository(filePath);
            var route = new TravelRoute("GRU", "BRC", 10);

            // Act
            repository.AddRoute(route);

            // Assert
            var routes = repository.GetAllRoutes();
            Assert.Single(routes);
            Assert.Equal("GRU", routes[0].Origin);
            Assert.Equal("BRC", routes[0].Destination);
            Assert.Equal(10, routes[0].Cost);
        }

        [Fact]
        public void GetAllRoutes_ShouldReturnAllPersistedRoutes()
        {
            // Arrange
            var filePath = "test_routes.txt";
            if (File.Exists(filePath)) File.Delete(filePath);

            var repository = new RouteRepository(filePath);
            repository.AddRoute(new TravelRoute("GRU", "BRC", 10));
            repository.AddRoute(new TravelRoute("BRC", "SCL", 5));

            // Act
            var routes = repository.GetAllRoutes();

            // Assert
            Assert.Equal(2, routes.Count);
        }
    }
}
