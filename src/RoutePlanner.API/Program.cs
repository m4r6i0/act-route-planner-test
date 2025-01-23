using Microsoft.EntityFrameworkCore;
using RoutePlanner.API.Configurations;
using RoutePlanner.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Configura o contexto do EF Core e outras dependências
builder.Services.ConfigureDependencies();

// Configuração padrão da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Adicione o Middleware Global para Tratamento de Exceções
app.UseMiddleware<RoutePlanner.API.Middleware.ExceptionMiddleware>();

// Inicializar o banco de dados automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RoutePlannerDbContext>();
    dbContext.Database.Migrate();
}

// Configuração padrão do middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Tornar o Program acessível para os testes
public partial class Program { }
