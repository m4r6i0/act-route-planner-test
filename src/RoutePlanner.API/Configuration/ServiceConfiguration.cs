using RoutePlanner.Domain.Interfaces;
using RoutePlanner.Domain.Entities;
using RoutePlanner.API.Repositories;
using RoutePlanner.API.Services;
using Microsoft.EntityFrameworkCore;
using RoutePlanner.API.Data;

namespace RoutePlanner.API.Configurations
{
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Configura as dependências da aplicação.
        /// </summary>
        /// <param name="services">O IServiceCollection para registrar dependências.</param>
        /// <param name="DbContextOptionsBuilder">Configurador de contextos.</param>
        public static void ConfigureDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptions = null)
        {
            ConfigureDatabaseOptions(services, dbContextOptions);

            // Registro do repositório genérico
            services.AddScoped<IRepository<TravelRoute>, RouteRepository>();

            // Registro do serviço
            services.AddScoped<RouteService>();
        }

        /// <summary>
        /// Configura bases de dados local para testes/prod.
        /// </summary>
        /// <param name="services">O IServiceCollection para registrar dependências.</param>
        /// <param name="DbContextOptionsBuilder">Configurador de contextos.</param>
        static void ConfigureDatabaseOptions(IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptions)
        {
            // Verifica se uma configuração de DbContext foi fornecida
            if (dbContextOptions == null)
            {
                // Configuração padrão para produção com SQLite
                var databasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database/RoutePlanner.db");

                // Criação do diretório se necessário
                var directory = Path.GetDirectoryName(databasePath);
                CheckDatabaseDirectory(directory);

                services.AddDbContext<RoutePlannerDbContext>(options =>
                    options.UseSqlite($"Data Source={databasePath}"));
            }
            else
            {
                // Usa a configuração fornecida (geralmente em testes)
                services.AddDbContext<RoutePlannerDbContext>(dbContextOptions);
            }
        }

        static void CheckDatabaseDirectory(string? directory)
        {
            if (string.IsNullOrEmpty(directory)) return;

            try
            {
                // Cria o diretório se não existir
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    return;
                }

                // Limpa o conteúdo do diretório
                foreach (var file in Directory.GetFiles(directory))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed database in Directory: {directory}. Details: {ex.Message}");
            }
        }

    }
}
