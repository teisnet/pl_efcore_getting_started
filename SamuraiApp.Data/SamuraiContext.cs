using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
	public class SamuraiContext : DbContext
	{
		public SamuraiContext(DbContextOptions<SamuraiContext> options) : base(options)
		{
			// Disable tracking
			// AsNoTracking returns a query, not a DbSet. DbSet methods like 'Find' won't be available.
			ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
		}

		public DbSet<Samurai> Samurais { get; set; }
		public DbSet<Quote> Quotes { get; set; }
		public DbSet<Clan> Clans { get; set; }
		// The Battle and Horse database tables will be created regardless the DBSet declaration, due to the relationships.
		// Though, in order to interact directly with the Battles, a DBSet is needed.
		public DbSet<Battle> Battles { get; set; }
		public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// This tells EF Core that SamuraiBattle is the many-to-many joint table for the Samurai and Battle tables.
			modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
			modelBuilder.Entity<Horse>().ToTable("Horses");
			// Views typically have no keys (?).
			// When 'HasNoKey' is used, the objects will not be tracked, and is read-only (I believe)
			// The view has been created manually changing the migration, so use 'ToView' to tell EF.
			// (EF Core doesn't know how to create views)
			modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");
		}
	}
}
