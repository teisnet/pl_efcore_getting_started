using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tests
{
	[TestClass]
	public class InMemoryTests
	{
		[TestMethod]
		public void CanInsertSamuraiIntoDatabase()
		{
			var builder = new DbContextOptionsBuilder();

			// Passing a name so the db can be used elsewhere using 'UseInMemoryDatabase' again.
			builder.UseInMemoryDatabase("CanInsertSamurai");

			using (var context = new SamuraiConsoleContext(builder.Options))
			{
				// context.Database.EnsureDeleted(); // Takes long time
				// 'EnsureCreated' can be used to seed the database with the EF Core migrations
				// context.Database.EnsureCreated();
				var samurai = new Samurai();
				context.Samurais.Add(samurai);
				// Properties are non-nullable needs to be populated in advance, eg. the 'Name' field.
				// Because the InMemory database is used, the samurai already gets its id when using 'Add', not 'SaveChanges'
				// 'SaveChanges' only make sense when testing with a real databse.
				// Use 'Samurais.Count' to check if added or:
				Assert.AreEqual(EntityState.Added, context.Entry(samurai).State);
			}
		}
	}
}
