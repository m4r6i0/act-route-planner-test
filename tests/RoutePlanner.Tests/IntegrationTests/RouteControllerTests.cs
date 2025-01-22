using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace RoutePlanner.Tests.IntegrationTests
{
    public class RouteControllerTests : IClassFixture<WebApplicationFactory<RoutePlanner.API.Program>>
    {
        private readonly HttpClient _client;

        public RouteControllerTests(WebApplicationFactory<RoutePlanner.API.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterRoute_ShouldReturnSuccess()
        {
            // Arrange
            var route = new { Origin = "GRU", Destination = "BRC", Cost = 10 };
            var content = new StringContent(JsonSerializer.Serialize(route), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/routes/register", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetBestRoute_ShouldReturnCheapestRoute()
        {
            // Arrange
            var route1 = new { Origin = "GRU", Destination = "BRC", Cost = 10 };
            var route2 = new { Origin = "BRC", Destination = "SCL", Cost = 5 };

            await _client.PostAsync("/api/routes/register", new StringContent(JsonSerializer.Serialize(route1), Encoding.UTF8, "application/json"));
            await _client.PostAsync("/api/routes/register", new StringContent(JsonSerializer.Serialize(route2), Encoding.UTF8, "application/json"));

            // Act
            var response = await _client.GetAsync("/api/routes/best?origin=GRU&destination=SCL");
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("GRU - BRC - SCL", result);
        }
    }
}
