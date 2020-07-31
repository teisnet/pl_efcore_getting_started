using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
	public class SamuraiContext : DbContext
	{
		public DbSet<Samurai> Samurais { get; set; }
		public DbSet<Quote> Quotes { get; set; }
		public DbSet<Clan> Clans { get; set; }
		// The Battle database table will be created regardless the DBSet declaration, due to the relationships.
		// Though, in order to interact directly with the Battles, a DBSet is needed.
		public DbSet<Battle> Battles { get; set; }


		public static readonly ILoggerFactory ConsoleLoggerFactory = LoggerFactory.Create(builder => {
			builder
				.AddFilter((category, level) =>
					category == DbLoggerCategory.Database.Command.Name
					&& level == LogLevel.Information
				)
				.AddConsole();
		});

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder
				.UseLoggerFactory(ConsoleLoggerFactory)
				.EnableSensitiveDataLogging() // Log parameter values
				.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = SamuraiAppData");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// This tells EF Core that SamuraiBattle is the many-to-many joint table for the Samurai and Battle tables.
			modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
			modelBuilder.Entity<Horse>().ToTable("Horses");
		}
	}
}
