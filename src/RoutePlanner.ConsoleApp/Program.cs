using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RoutePlanner.Domain.Entities;


var configuration = LoadConfiguration();

var apiUrl = configuration["ApiBaseUrl"];
var defaultRoute = configuration["DefaultRoute"];

Console.Clear();
Console.WriteLine($"Bem-vindo ao RoutePlanner Console! (Rota padrão: {defaultRoute ?? "Nenhuma"})");

while (true)
{
    Console.Clear();
    Console.WriteLine("\nSelecione uma opção:");
    Console.WriteLine("1. Inserir rotas pré-cadastradas");
    Console.WriteLine("2. Inserir uma nova rota");
    Console.WriteLine("3. Consultar a melhor rota");
    Console.WriteLine("4. Sair");
    Console.Write("Opção: ");
    var option = Console.ReadLine();

    switch (option)
    {
        case "1":
          await SeedRoutes(apiUrl, configuration);
          
          break;
        case "2":
            await InsertRoute(apiUrl);
            break;
        case "3":
            await GetBestRoute(apiUrl, defaultRoute);
            break;
        case "4":
            Console.WriteLine("Saindo...");
            return;
        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}

static IConfiguration LoadConfiguration()
{
    var basePath = AppDomain.CurrentDomain.BaseDirectory;

    return new ConfigurationBuilder()
        .SetBasePath(basePath) // Define o caminho base como o diretório atual da aplicação
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Carrega o appsettings.json
        .Build();
}


static async Task InsertRoute(string apiUrl)
{
    Console.Write("Digite a origem: ");
    var origin = Console.ReadLine();

    Console.Write("Digite o destino: ");
    var destination = Console.ReadLine();

    Console.Write("Digite o custo: ");
    if (!int.TryParse(Console.ReadLine(), out var cost))
    {
        Console.WriteLine("Custo inválido. Tente novamente.");
        return;
    }

    var route = new
    {
        Origin = origin,
        Destination = destination,
        Cost = cost
    };

    var client = new HttpClient();
    var content = new StringContent(JsonSerializer.Serialize(route), Encoding.UTF8, "application/json");

    var response = await client.PostAsync($"{apiUrl}/routes/register", content);

    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Rota registrada com sucesso!");
    }
    else
    {
        var errorMessage = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro ao registrar rota: {response.StatusCode} - {errorMessage}");
    }
}

static async Task GetBestRoute(string apiUrl, string defaultRoute)

{
    Console.WriteLine("Digite a rota (ex: GRU-CDG) ou pressione ENTER para usar a rota padrão:");
    var input = Console.ReadLine();

    if (string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(defaultRoute))
    {
        input = defaultRoute;
    }

    var routeParts = input?.Split('-');
    if (routeParts?.Length == 2)
    {
        var origin = routeParts[0];
        var destination = routeParts[1];

        var client = new HttpClient();
        var response = await client.GetAsync($"{apiUrl}/routes/best?origin={origin}&destination={destination}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Melhor Rota: {result}");
        }
        else
        {
            Console.WriteLine($"Erro ao consultar rota: {response.StatusCode}");
        }
    }
    else
    {
        Console.WriteLine("Formato inválido!");
    }
}

static async Task SeedRoutes(string apiUrl, IConfiguration configuration)
{
    var seedRoutes = configuration.GetSection("SeedRoutes").Get<List<TravelRoute>>();
    
    if (seedRoutes == null || seedRoutes.Count == 0)
    {
        Console.WriteLine("Nenhuma rota para semear encontrada no arquivo de configuração.");
        return;
    }

    Console.WriteLine("\nSemear rotas no banco de dados...");

    var client = new HttpClient();

    foreach (var route in seedRoutes)
    {
        var content = new StringContent(JsonSerializer.Serialize(route), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{apiUrl}/routes/register", content);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Rota {route.Origin} -> {route.Destination} com custo {route.Cost} registrada com sucesso!");
        }
        else
        {
            Console.WriteLine($"Erro ao registrar rota {route.Origin} -> {route.Destination}: {response.StatusCode}");
        }
    }
}