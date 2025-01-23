using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using RoutePlanner.API.Data;
using RoutePlanner.API.Models.Requests;
using System.Net.Http.Json;
using System.Text.Json;
using RoutePlanner.Tests.Helpers;

namespace RoutePlanner.Tests.IntegrationTests
{
    public class RouteControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RouteControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Usa a InMemoryDbContextFactory para configurar o banco de dados
                    using var context = InMemoryDbContextFactory.CreateDbContext();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                });
            }).CreateClient();
        }

        [Fact]
        public async Task RegisterRoute_ShouldReturnSuccess()
        {
            // Arrange
            var route = new TravelRouteRequest
            {
                Origin = "GRU",
                Destination = "BRC",
                Cost = 10
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/routes/register", route);

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Route registered successfully!", content);
        }

        [Fact]
        public async Task RegisterRoute_InvalidInput_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidRoute = new { Origin = "GRU", Destination = "", Cost = -10 };

            // Act
            var response = await _client.PostAsJsonAsync("/api/routes/register", invalidRoute);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetBestRoute_ShouldReturnCorrectRoute()
        {
            // Arrange
            var route1 = new { Origin = "GRU", Destination = "BRC", Cost = 10 };
            var route2 = new { Origin = "BRC", Destination = "SCL", Cost = 5 };
            await _client.PostAsJsonAsync("/api/routes/register", route1);
            await _client.PostAsJsonAsync("/api/routes/register", route2);

            // Act
            var response = await _client.GetAsync("/api/routes/best?origin=GRU&destination=SCL");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("GRU - BRC - SCL", content);
        }

        [Fact]
        public async Task GetBestRoute_NoRoute_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/routes/best?origin=GRU&destination=XYZ");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
