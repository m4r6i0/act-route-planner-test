using RoutePlanner.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Carregar o caminho do arquivo de rotas
var routesFilePath = builder.Configuration["RoutesDataFile"] ?? "routes.txt";

// Configurar dependências
builder.Services.ConfigureDependencies(routesFilePath);

// Configurar serviços padrão
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
