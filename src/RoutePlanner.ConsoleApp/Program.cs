using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

var apiUrl = "https://localhost:5001/api/routes";

Console.WriteLine("Digite a rota (ex: GRU-CDG):");
var input = Console.ReadLine()?.Split('-');
if (input?.Length == 2)
{
    var origin = input[0];
    var destination = input[1];

    var client = new HttpClient();
    var response = await client.GetStringAsync($"{apiUrl}/best?origin={origin}&destination={destination}");

    Console.WriteLine($"Melhor Rota: {response}");
}
else
{
    Console.WriteLine("Formato inválido!");
}
