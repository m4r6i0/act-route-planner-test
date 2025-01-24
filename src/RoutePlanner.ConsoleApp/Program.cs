using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RoutePlanner.Domain.Entities;

var configuration = LoadConfiguration();
var apiUrl = configuration["ApiBaseUrl"];
var defaultRoute = configuration["DefaultRoute"];
ShowSplash(defaultRoute);

while (true)
{

    ShowMenu();
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
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Saindo... Obrigado por usar o RoutePlanner!");
            Console.ResetColor();
            return;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Opção inválida. Tente novamente.");
            Console.ResetColor();
            break;
    }

    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
    Console.ReadKey();
    Console.Clear();
    ShowSplash(defaultRoute);

}

static void ShowSplash(string defaultRoute)
{

    // Arte em ASCII no título
    var title = @"

 ____  ____  _     _____  _____ ____    ____  _     ____  _      _      _____ ____ 
/  __\/  _ \/ \ /\/__ __\/  __//  __\  /  __\/ \   /  _ \/ \  /|/ \  /|/  __//  __\
|  \/|| / \|| | ||  / \  |  \  |  \/|  |  \/|| |   | / \|| |\ ||| |\ |||  \  |  \/|
|    /| \_/|| \_/|  | |  |  /_ |    /  |  __/| |_/\| |-||| | \||| | \|||  /_ |    /
\_/\_\\____/\____/  \_/  \____\\_/\_\  \_/   \____/\_/ \|\_/  \|\_/  \|\____\\_/\_\
                                                                                   
                                                                                       
";
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(title);
    Console.ResetColor();
    Console.WriteLine($"Bem-vindo ao RoutePlanner Console! (Rota padrão: {defaultRoute ?? "Nenhuma"})");
}

static void ShowMenu()
{
    //Console.Clear();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("========= MENU =========");
    Console.WriteLine("1. Inserir rotas pré-cadastradas");
    Console.WriteLine("2. Inserir uma nova rota");
    Console.WriteLine("3. Consultar a melhor rota");
    Console.WriteLine("4. Sair");
    Console.WriteLine("========================");
    Console.ResetColor();
    Console.Write("Escolha uma opção: ");
}

static IConfiguration LoadConfiguration()
{
    var basePath = AppDomain.CurrentDomain.BaseDirectory;

    return new ConfigurationBuilder()
        .SetBasePath(basePath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Custo inválido. Tente novamente.");
        Console.ResetColor();
        return;
    }

    var route = new { Origin = origin, Destination = destination, Cost = cost };

    using var client = new HttpClient();
    var content = new StringContent(JsonSerializer.Serialize(route), Encoding.UTF8, "application/json");
    var response = await client.PostAsync($"{apiUrl}/routes/register", content);

    if (response.IsSuccessStatusCode)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Rota registrada com sucesso!");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        var errorMessage = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Erro ao registrar rota: {response.StatusCode} - {errorMessage}");
    }
    Console.ResetColor();
}


static async Task SeedRoutes(string apiUrl, IConfiguration configuration)
{
    var seedRoutes = configuration.GetSection("SeedRoutes").Get<List<TravelRoute>>();

    if (seedRoutes == null || seedRoutes.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Nenhuma rota para semear encontrada no arquivo de configuração.");
        Console.ResetColor();
        return;
    }

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("\nSemear rotas no banco de dados...");
    Console.ResetColor();

    using var client = new HttpClient();
    foreach (var route in seedRoutes)
    {
        var content = new StringContent(JsonSerializer.Serialize(route), Encoding.UTF8, "application/json");
        var response = await client.PostAsync($"{apiUrl}/routes/register", content);

        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Rota {route.Origin} -> {route.Destination} com custo {route.Cost} registrada com sucesso!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro ao registrar rota {route.Origin} -> {route.Destination}: {response.StatusCode}");
        }
        Console.ResetColor();
    }
}

static async Task GetBestRoute(string apiUrl, string defaultRoute)
{
    Console.Clear();
    Console.WriteLine("===== CONSULTAR MELHOR ROTA =====");

    // Solicita a origem
    string origin = "";
    while (string.IsNullOrEmpty(origin))
    {
        Console.Write("Digite o ponto de origem (ex: GRU): ");
        origin = Console.ReadLine()?.Trim().ToUpper();

        if (string.IsNullOrEmpty(origin))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Origem não pode estar vazia. Tente novamente.");
            Console.ResetColor();
        }
    }

    // Solicita o destino
    string destination = "";
    while (string.IsNullOrEmpty(destination))
    {
        Console.Write("Digite o ponto de destino (ex: CDG): ");
        destination = Console.ReadLine()?.Trim().ToUpper();

        if (string.IsNullOrEmpty(destination))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Destino não pode estar vazio. Tente novamente.");
            Console.ResetColor();
        }
    }

    // Confirmação para o uso da rota padrão, se disponível
    if (string.IsNullOrEmpty(origin) && string.IsNullOrEmpty(destination) && !string.IsNullOrEmpty(defaultRoute))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Nenhuma rota foi informada. Usando rota padrão: {defaultRoute}");
        Console.ResetColor();

        var routeParts = defaultRoute.Split('-');
        origin = routeParts[0];
        destination = routeParts[1];
    }

    // Validação final antes de fazer a chamada
    if (origin.Equals(destination, StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Origem e destino não podem ser iguais. Tente novamente.");
        Console.ResetColor();
        return;
    }

    Console.WriteLine($"Consultando a melhor rota de {origin} para {destination}...");

    // Faz a chamada para a API
    try
    {
        using var client = new HttpClient();
        var response = await client.GetAsync($"{apiUrl}/routes/best?origin={origin}&destination={destination}");

        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"\nMelhor Rota Encontrada: {result}");
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nNenhuma rota encontrada para os pontos informados.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nErro ao consultar rota: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nOcorreu um erro inesperado: {ex.Message}");
    }
    finally
    {
        Console.ResetColor();
    }

    // Pausa para permitir que o usuário veja a mensagem
    Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
    Console.ReadKey();
}
