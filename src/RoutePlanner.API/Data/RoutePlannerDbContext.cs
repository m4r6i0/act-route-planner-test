using Microsoft.EntityFrameworkCore;
using RoutePlanner.Domain.Entities;
using System.IO;

namespace RoutePlanner.API.Data
{
    public class RoutePlannerDbContext : DbContext
    {
        public DbSet<TravelRoute> TravelRoutes { get; set; }

        // Construtor para aceitar DbContextOptions (necessário para testes e flexibilidade)
        public RoutePlannerDbContext(DbContextOptions<RoutePlannerDbContext> options)
            : base(options)
        {
        }

        // Construtor padrão para uso com a configuração atual
        public RoutePlannerDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var databaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "database");

                if (!Directory.Exists(databaseDirectory))
                {
                    Directory.CreateDirectory(databaseDirectory);
                }

                // Define o caminho do banco de dados na pasta Data
                var databasePath = Path.Combine(databaseDirectory, "RoutePlanner.db");
                optionsBuilder.UseSqlite($"Data Source={databasePath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TravelRoute>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Origin).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Destination).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cost).IsRequired();
            });
        }
    }
}
