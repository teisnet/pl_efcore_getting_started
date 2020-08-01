using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
	class Program
	{
		/*
		 * Execution methods;
		 * ToList()
		 * FirstOrDeault()
		 * Find()?
		 * Single()?
		 */

		private static SamuraiContext context = new SamuraiContext();
		static void Main(string[] args)
		{
			// context.Database.EnsureCreated();

			#region Simple objects
			// GetSamurais("Before add");
			// AddSamurai();
			// GetSamurais("After add");

			// InsertMultipleSamurais();
			// InstertVariousTypes();
			// GetSamuraisSimpler();
			// QueryFilters();
			// RetrieveAndUpdateSamurai();
			// RetrieveAndUpdateMultipleSamurai();
			// MultipleDatabaseOperations();
			// RetrieveAndDeleteSamurai();
			// GetSamurais("After add multiple");

			// InsertBattle();
			// QueryAndUpdateBattle_Disconnected();
			#endregion

			#region Related data
			// InsertNewSamuraiWithAQuote();
			// InsertNewSamuraiWithManyQuotes();
			// AddingQuoteToExistingSamuraiWhileTracked();
			// AddingQuoteToExistingSamuraiWhileNotTracked(19);
			// AddingQuoteToExistingSamuraiWhileNotTracked_Easy(19);
			EagerLoadSamuraiWithQuotes();
			#endregion
		}

		#region Related data

		private static void EagerLoadSamuraiWithQuotes()
		{
			// var samuraiWithQuotes = context.Samurais.Include(s => s.Quotes).ToList();

			// 'Include' is a member of DBSet and can't be _after_ an execution method like 'FirstOrDefault'.
			// Invalid: context.Samurais.FirstOrDefault(s => s.Name.Contains("Kambei")).Include(s => s.Quotes);
			var samuraiWithQuotes = context.Samurais
										.Where(s => s.Name.Contains("Kambei"))
										.Include(s => s.Quotes)
										// This pics the first _samurai_ NOT the first quote.
										.FirstOrDefault();
										// .ToList();

			// IMPORTANT: Include() does now allow to filter _which_ related data
			// is returned. It always loads the entire set of related objects.

			// Various combinations

			// Include child objects
			// context.Samurais.Include(s => s.Quotes)

			// Include children and grandchildren:
			// context.Samurais.Include(s => s.Quotes).ThenInclude(q => q.Translations)

			// Include just grandchildren
			// Teis question: Where are the translation collection added?
			// context.Samurais.Include(s => s.Quotes.Translations)

			// Include different children
			// context.Samurais.Include(s => s.Quotes)
			//				   .Include(s => s.Clan)

		}

		private static void InsertNewSamuraiWithAQuote()
		{
			var samurai = new Samurai
			{
				Name = "Kambei Shimada",
				Quotes = new List<Quote>
				{
					new Quote { Text = "I've come to save you" }
				}
			};
			context.Samurais.Add(samurai);
			context.SaveChanges();
		}
		private static void InsertNewSamuraiWithManyQuotes()
		{
			var samurai = new Samurai
			{
				Name = "Kambei Shimada",
				Quotes = new List<Quote>
				{
						new Quote { Text = "I've come to save you" },
						new Quote { Text = "I told you to watch out for the sharp sword! Oh well!" }	
				}
			};
			context.Samurais.Add(samurai);
			context.SaveChanges();
		}

		private static void AddingQuoteToExistingSamuraiWhileTracked()
		{
			var samurai = context.Samurais.FirstOrDefault();

			// It is important that the context is still tracked when adding quotes.
			samurai.Quotes.Add(new Quote
			{
				Text = "I bet you're happy that I've saved you!"
			});
			context.SaveChanges();
		}


		private static void AddingQuoteToExistingSamuraiWhileNotTracked(int samuraiId)
		{
			var samurai = context.Samurais.Find(samuraiId);

			samurai.Quotes.Add(new Quote
			{
				Text = "Now that I saved you, will you feed me dinner?"
			});

			using (var newContext = new SamuraiContext())
			{
				// Use 'Update' to start tracking the graph.
				// As child's key value is not set state will automatically be set as 'added'
				// Child's FK valueto parent, 'samuraiId',  is set to parent\s key.

				// Using Update
				// Update will also make a db call to update the samurai, although no changes has been made
				// newContext.Samurais.Update(samurai);

				// Using Attach
				// Use 'attach' instead when adding related objects, not editing the parent itself.
				// This sets it state to 'unmodified' but still recognized
				// the missing key and fk in the quote object.

				// ...or just set the fk on the Samurai (se next method)
				newContext.Samurais.Attach(samurai);

				newContext.SaveChanges();
			}

		}

		private static void AddingQuoteToExistingSamuraiWhileNotTracked_Easy(int samuraiId)
		{
			var quote = new Quote
			{
				Text = "Now that I saved you, will you feed me dinner?",
				// Somewhat 'unclean', but makes everything easier.
				SamuraiId = samuraiId
			};

			using (var newContext = new SamuraiContext())
			{
				// ...or just set the fk on the Samurai (se next method)
				newContext.Quotes.Add(quote);
				newContext.SaveChanges();
			}

		}
		#endregion

		#region Simple objects
		private static void QueryAndUpdateBattle_Disconnected()
		{
			// AsNoTracking returns a query, not a DbSet. DbSet methods like 'Find' won't be available.
			var battle = context.Battles.AsNoTracking().FirstOrDefault();
			battle.EndDate = new DateTime(1560, 06, 30);

			using (var newContextInstance = new SamuraiContext())
			{
				newContextInstance.Battles.Update(battle);
				newContextInstance.SaveChanges();
			}
		}

		private static void InsertBattle()
		{
			context.Battles.Add(new Battle {
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

		#endregion
	}
}
