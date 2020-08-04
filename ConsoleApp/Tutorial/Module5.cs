using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp.Tutorial
{
	public class Module5Simple
	{
		/* Module 5 - Interact With the EF Core Data Model
		 * (Simple objects)
		 *
		 *
		 * All Execution methods:
		 * (all methods have async counterparts)
		 *
		 * LINQ to entities execution methods:
		 * ToList()
		 * First()
		 * FirstOrDeault()
		 * Single()
		 * SingleOrDefault()
		 * Last() *
		 * LastOrDefault() *
		 * Count()
		 * LongCount()
		 * Min(), Max()
		 * Avgerage(), Sum()
		 * AsAsyncEnumerable (async only)
		 *
		 * Not a LINQ method but a DbSet method that will execute:
		 * Find()   (Returns object immediately if already tracked in memory)
		 * 
		 * *) 'Last' methods require to have an OrderBy() method
		 *    otherwise they will return full set then pick last in memory.
		 *
		 *
		 * DbSet and DbContext commands
		 * (the '-Range' methods of DbContext accept multiple entity types)
		 * Add, AddRange
		 * Update, UpdateRange (Context will start tracking the object and mark its state as 'modified'. Teis: I think 'Update' is only used in disconnected scenarios)
		 * Remove, RemoveRange
		 */

		private static SamuraiContext context = new SamuraiContext();

		public static void Run()
		{
			// 5.4 - Benefit from bulk operations support
			// InsertMultipleSamurais();
			// InstertVariousTypes();

			// 5.5 - Understand the query workflow
			// GetSamuraisSimpler();

			// 5.6 - Filtering in queries
			// 5.7 - Aggregating in queries
			// QueryFilters();

			// 5.8 - Update simple objects
			// RetrieveAndUpdateSamurai();
			// RetrieveAndUpdateMultipleSamurais();
			// MultipleDatabaseOperations();

			// 5.9 - Delete simple objects
			// RetrieveAndDeleteSamurai();

			// 5.10 - Persist data in disconnected scenarios
			// InsertBattle();
			// QueryAndUpdateBattle_Disconnected();


		}

		private static void QueryAndUpdateBattle_Disconnected()
		{
			// AsNoTracking returns a query, not a DbSet. DbSet methods like 'Find' won't be available.
			var battle = context.Battles.AsNoTracking().FirstOrDefault();
			battle.EndDate = new DateTime(1560, 06, 30);

			using (var newContextInstance = new SamuraiContext())
			{
				// Update: Context will start tracking the object and mark its state as 'modified'
				// Teis: I think 'Update' is only used in disconnected scenarios
				newContextInstance.Battles.Update(battle);
				newContextInstance.SaveChanges();
			}
		}

		private static void InsertBattle()
		{
			context.Battles.Add(new Battle
			{
				Name = "Battle of Okehazma",
				StartDate = new DateTime(1560, 05, 01),
				EndDate = new DateTime(1560, 06, 15)
			});
			context.SaveChanges();
		}

		private static void RetrieveAndDeleteSamurai()
		{
			var samurai = context.Samurais.Find(3);
			context.Samurais.Remove(samurai);
			context.SaveChanges();

			// Not recommended: It would be possible to trick EF by createing fake objects
			// with only the id of the object to be deleted.
			// Consider using stored procedures for this, instead of tricking EF.
		}

		private static void MultipleDatabaseOperations()
		{
			// Multiple different operations will be persisted with SaveChanges
			var samurai = context.Samurais.FirstOrDefault();
			samurai.Name += "San";
			context.Samurais.Add(new Samurai { Name = "Kikuchiyo" });
			context.SaveChanges();
		}

		private static void RetrieveAndUpdateMultipleSamurais()
		{
			var samurais = context.Samurais.Skip(1).Take(4).ToList();
			samurais.ForEach(s => s.Name += "San");
			context.SaveChanges();
		}

		private static void RetrieveAndUpdateSamurai()
		{
			var samurai = context.Samurais.FirstOrDefault();
			samurai.Name += "San";
			context.SaveChanges();
		}

		private static void QueryFilters()
		{
			var name = "Sampson";

			// 1 - Where
			// var samuraiSampson = context.Samurais.Where(s => s.Name == "Sampson").ToList(); // Value is harcoded into SQL.
			var samuraiSampson = context.Samurais.Where(s => s.Name == name).ToList(); // Value is passed as a parameter to SQL.

			// 2 - Like (StartsWith, etc.)
			var samuraiLike = context.Samurais.Where(s => EF.Functions.Like(s.Name, "J%")).ToList();
			var samuraisStartsWith = context.Samurais.Where(s => s.Name.StartsWith("J")).ToList(); // Also translates to a SQL 'LIKE* statement.

			// 3 - FirstOrDefault
			var samuraiByName = context.Samurais.FirstOrDefault(s => s.Name == name);
			// Same as:  context.Samurais.Where(s => s.Name == name).FirstOrDefault();
			// Though the same is not possible with 'ToList'

			// 4 - Find
			// 'Find' is a DbSet and not LINQ method, but will also execute.
			var samuraiById = context.Samurais.Find(2); // Can find objects already tracked in memory.

			// 5 - LastOrDefault (+ OrderBy)
			// If 'OrderBy' is omitted, the code will always throw an exception at runtime.
			var lastSamurai = context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);
		}

		private static void GetSamuraisSimpler()
		{
			// var samurais = context.Samurais.ToList(); // Recommended
			var query = context.Samurais;

			var samurais = query.ToList(); // Append a Linq execution method, before enumerating.

			// Unless ToList() is not called already (using the 'query' variable), the database call will be executed at
			// foreach call and the connection will stay open until the enumeration is complete.
			foreach (var samurai in samurais /*query (not recommended)*/)
			{
				Console.WriteLine(samurai.Name);
			}
		}

		private static void InstertVariousTypes()
		{
			var samurai = new Samurai { Name = "Kikuchio" };
			var clan = new Clan { ClanName = "Imperial Clan" };
			context.AddRange(samurai, clan);
			context.SaveChanges();
		}

		private static void InsertMultipleSamurais()
		{
			var samurai = new Samurai { Name = "Sampson" };
			var samurai2 = new Samurai { Name = "Tasha" };
			var samurai3 = new Samurai { Name = "Sarah" };
			var samurai4 = new Samurai { Name = "India" };
			// Can also be passed as a list
			context.Samurais.AddRange(samurai, samurai2, samurai3, samurai4);

			context.SaveChanges();
		}
	}
}
