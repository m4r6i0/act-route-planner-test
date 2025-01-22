using RoutePlanner.API.Models;
using RoutePlanner.API.Repositories;
using RoutePlanner.API.Services;
using System.Collections.Generic;
using Xunit;

namespace RoutePlanner.Tests.UnitTests.Services
{
    public class RouteServiceTests
    {
        [Fact]
        public void GetBestRoute_ShouldReturn_CheapestRoute()
        {
            // Arrange
            var repository = new RouteRepository("test_routes.txt");
            var service = new RouteService(repository);

            repository.AddRoute(new TravelRoute("GRU", "BRC", 10));
            repository.AddRoute(new TravelRoute("BRC", "SCL", 5));
            repository.AddRoute(new TravelRoute("GRU", "CDG", 75));
            repository.AddRoute(new TravelRoute("GRU", "SCL", 20));
            repository.AddRoute(new TravelRoute("SCL", "ORL", 20));
            repository.AddRoute(new TravelRoute("ORL", "CDG", 5));

            // Act
            var result = service.GetBestRoute("GRU", "CDG");

            // Assert
            Assert.Equal("GRU - BRC - SCL - ORL - CDG", result.route);
            Assert.Equal(40, result.cost);
        }

        [Fact]
        public void GetBestRoute_NoRouteExists_ShouldReturn_EmptyRoute()
        {
            // Arrange
            var repository = new RouteRepository("test_routes.txt");
            var service = new RouteService(repository);

            // Act
            var result = service.GetBestRoute("GRU", "CDG");

            // Assert
            Assert.Equal("", result.route);
            Assert.Equal(0, result.cost);
        }
    }
}
