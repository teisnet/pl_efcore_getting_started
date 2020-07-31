using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace ConsoleApp
{
	class Program
	{
		private static SamuraiContext context = new SamuraiContext();
		static void Main(string[] args)
		{
			// context.Database.EnsureCreated();
			// GetSamurais("Before add");
			// AddSamurai();
			// GetSamurais("After add");
			// InsertMultipleSamurais();
			// InstertVariousTypes();
			// GetSamuraisSimpler();
			// QueryFilters();
			// RetrieveAndUpdateSamurai();
			// RetrieveAndUpdateMultipleSamurai();
			MultipleDatabaseOperations();
			// GetSamurais("After add multiple");
			// Console.WriteLine("Press any key...");
			// Console.ReadKey();
		}

		private static void MultipleDatabaseOperations()
		{
			// Multiple different operations will be persisted with SaveChanges
			var samurai = context.Samurais.FirstOrDefault();
			samurai.Name += "San";
			context.Samurais.Add(new Samurai { Name = "Kikuchiyo" });
			context.SaveChanges();
		}

		private static void RetrieveAndUpdateMultipleSamurai()
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

			// 4 - Find
			var samuraiById = context.Samurais.Find(2); // Can find objects already tracked in memory.

			// 5 - LastOrDefault (+ OrderBy)
			// If OrderBy is omitted, the code will always throw an exception.
			var lastSamurai = context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);
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

		private static void AddSamurai()
		{
			var samurai = new Samurai{ Name = "Julie" };
			context.Samurais.Add(samurai);
			context.SaveChanges();
		}

		private static void GetSamurais(string text)
		{
			var samurais = context.Samurais.ToList();
			Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
			foreach (var samurai in samurais)
			{
				Console.WriteLine(samurai.Name);
			}

		}

		private static void GetSamuraisSimpler()
		{
			// var samurais = context.Samurais.ToList();
			var query = context.Samurais;
			
			var samurais = query.ToList(); // Optional but recommended.

			// Unless ToList() is not called in advanced, the database call will be executed at
			// foreach and the connection will stay open for the duration of the loop.
			foreach (var samurai in query)
			{
				Console.WriteLine(samurai.Name);
			}
		}
	}
}
