using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

			/** One-to-many relationships (Samurai-Quotes) **/

			// 6.2 - Insert related data
			// InsertNewSamuraiWithAQuote();
			// InsertNewSamuraiWithManyQuotes();
			// AddingQuoteToExistingSamuraiWhileTracked();
			// AddingQuoteToExistingSamuraiWhileNotTracked(19);
			// AddingQuoteToExistingSamuraiWhileNotTracked_Easy(19);

			// 6.3 - Eager load related data
			// EagerLoadSamuraiWithQuotes();

			// 6.4 - Project related data in queries
			// ProjectSomeProperties();
			// ProjectSamuraisWithQuotes();

			// 6.5 - Load related data for objects already in memory
			// ExplicitLoadQuotes();
			// LazyLoadQuotes();

			// 6.6 - Use related data to filter objects
			// FilteringWithRelatedData();

			// 6.7 - Modify related data
			// ModifyingRelatedDataWhenTracked();
			// ModifyingRelatedDataWhenNotTracked();

			/** Many-to-many relationships **/

			// 6.8 - Create and change many-to-many relationships
			// JoinBattleAndSamurai();
			// EnlistSamuraiIntoABattle();
			// RemoveJoinBetweenSamuraiAndBattleSimple();

			// 6.9 - Query across many-to-many relationships
			// GetSamuraiWithBattles();

			/** One-to-one relationships **/

			// 6.10 - Persist data in one-to-one relationships
			// AddNewSamuraiWithHorse();
			// AddNewHorseToSamuraiUsingId();
			// AddNewHorseToSamuraiObject();
			// AddNewHorseToDisconnectedSamuraiObject();
			// ReplaceAHorse();

			// 6.11 - Query one-to-one relationships
			// GetSamuraiWithHorse();
			GetHorseWithSamurai();

			#endregion
		}

		private static void GetHorseWithSamurai()
		{
			// Use 'context.Set<Horse>()' when a DbContext doesn't exist for the object type.
			var horseWithoutSamurai = context.Set<Horse>().Find(11);

			// Include
			// Get the samurai that has the horse with id=3.
			// This can be used when the Horse class doesn't have a Samurai navigation property.
			var horseWithSamurai = context.Samurais.Include(s => s.Horse)
				.FirstOrDefault(s => s.Horse.Id == 3);

			// Projection
			// Get an { Samurai, Horse } object list, only for samurais that have a horse.
			// Again, this can be used whern the Horse class doesn't have a Samurai navigation property.
			var horseWithSamurais = context.Samurais
				.Where(s => s.Horse != null)
				.Select(s => new { Horse = s.Horse, Samurai = s })
				.ToList();
		}

		private static void GetSamuraiWithHorse()
		{
			// Get all the samurais with their Horses list populated.
			var samurais = context.Samurais.Include(s => s.Horse).ToList();
		}

		private static void ReplaceAHorse()
		{
			// Horse already trached in memory:

			var samurai = context.Samurais.Include(s => s.Horse).FirstOrDefault(s => s.Id == 18); // Already has a horse

			/*
			Will throw, as the horse object needs to be in memory. EF will just send an insert to the db:
			samurai = context.Samurais.Find(23); // Already has a horse
			samurai.Horse = new Horse { Name = "Trigger" };
			*/

			// For 'horse trading' you can also set the horse's SamuraiId to the new samurai owner.

			// The db call will first delete the old horse and the insert the new horse,
			// because there can not be a horse without a samurai.
			context.SaveChanges();
		}

		private static void AddNewHorseToDisconnectedSamuraiObject()
		{
			var samurai = context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 18 );
			samurai.Horse = new Horse { Name = "Mr. Ed" };
			samurai.Name += " Draiby";
			using (var newContext = new SamuraiContext())
			{
				// Using the 'Attach' method, there os only one db call inserting into the Horses table.
				// Teis: What if the Samurai object HAS beed changed? Will it then be updated as well?
				// Teis: Answer: NO
				// Teis: Probably no if samurais had foreign horse keys as well.
				// EF Core sees that the samurai already has an id and will mark it as unchanged.
				// but because the horse doesn't have an id, so its state will be set to 'added'.
				newContext.Attach(samurai);
				newContext.SaveChanges();
			}
		}

		private static void AddNewHorseToSamuraiObject()
		{
			var samurai = context.Samurais.Find(12);
			samurai.Horse = new Horse { Name = "Black Beauty" };
			context.SaveChanges();
		}


		private static void AddNewHorseToSamuraiUsingId()
		{
			var horse = new Horse { Name = "Scout", SamuraiId = 22 };
			// Since there is no Horse DbSet the horse can be added directly to the context.
			context.Add(horse);
			context.SaveChanges();
		}

		private static void AddNewSamuraiWithHorse()
		{
			// EF will shrow an error if it already has a horse.
			// Solve this in the businiss logic.
			// Consider also adding a foreign 'Horse' key in the 'Samurai' class.
			// Results in two db calls, one for the samurai and one for the horse using the new samuraiId.
			var samurai = new Samurai { Name = "Jina Ujichika" };
			samurai.Horse = new Horse { Name = "Silver" };
			context.Samurais.Add(samurai);
			context.SaveChanges();
		}

		private static void GetSamuraiWithBattles()
		{
			// Creates one db call fetching samurai 19 and related data
			// Using 'Include' unfortunately we have to go through all samuraibattles to finally retrieve the battles.
			/* var samuraiWithBattles = context.Samurais
				.Include(s => s.SamuraiBattles)
				.ThenInclude(b => b.Battle)
				.FirstOrDefault(samurai => samurai.Id == 19);
			*/

			// Use projection instead:
			var samuraiWithBattles = context.Samurais.Where(s => s.Id == 19)
				.Select(s => new
				{
					Samurai = s,
					Battles = s.SamuraiBattles.Select( sb => sb.Battle)
				})
				.FirstOrDefault(); // Teis: Is this on the samurai level?
		}

		private static void RemoveJoinBetweenSamuraiAndBattleSimple()
		{
			var join = new SamuraiBattle { BattleId = 1, SamuraiId = 11 };
			// Begin tracking and set state as deleted (only the delete call is performed on the database).
			context.Remove(join);
			context.SaveChanges();
		}

		private static void EnlistSamuraiIntoABattle()
		{
			var battle = context.Battles.Find(1);
			// The change tracker automatically adds the BattleId.
			battle.SamuraiBattles.Add(new SamuraiBattle { SamuraiId = 11 });
			context.SaveChanges();
		}

		private static void JoinBattleAndSamurai()
		{
			// Samurai and Battle already exist and we have their id's.
			var sbJoin = new SamuraiBattle { SamuraiId = 11, BattleId = 1 };
			// Note, there is no DbSet for the SamuraiBattle class.
			context.Add(sbJoin);
			context.SaveChanges();
		}

		private static void ModifyingRelatedDataWhenNotTracked()
		{
			var samurai = context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 19);
			var quote = samurai.Quotes[0];
			quote.Text = "Did you hear that again?";

			using (var newContext = new SamuraiContext())
			{
				// Don't use this. It will update the parent samurai and all its quotes.
				// Don't use: newContext.Quotes.Update(quote);
				// ...use this instead. Will only modify the quote in the database.
				newContext.Entry(quote).State = EntityState.Modified;
				newContext.SaveChanges();

			}
		}

		private static void ModifyingRelatedDataWhenTracked()
		{
			var samurai = context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 19);
			samurai.Quotes[0].Text = "Did you hear that?";
			context.Quotes.Remove(samurai.Quotes[2]);
			context.SaveChanges();
		}

		private static void FilteringWithRelatedData()
		{
			// Use the related data to filter or sort the main object.
			// This results in only one query.
			// Other LINQ queries that 'Where' can be used.
			var samurais = context.Samurais
				.Where(s => s.Quotes.Any(q => q.Text.Contains("save")))
				.ToList();
		}

		private static void LazyLoadQuotes()
		{
			// This will only work with lazy loading enabled, which is NOT recommended.
			// Lazy loading is disabled by default.
			var samurai = context.Samurais.FirstOrDefault(s => s.Name.Contains("Kikuchiyo"));
			var quoteCount = samurai.Quotes.Count();
		}

		private static void ExplicitLoadQuotes()
		{
			var samurai = context.Samurais.FirstOrDefault(s => s.Name.Contains("Kikuchiyo"));
			// Load only works for a single object
			context.Entry(samurai).Collection(s => s.Quotes).Load();
			context.Entry(samurai).Reference(s => s.Horse).Load();

			/*
			// Filter loaded data using 'Query'. This only results in one db execution.
			// (Not possible with eager loading)
			var happyQuotes = context.Entry(samurai)
				.Collection(b => b.Quotes)
				.Query()
				.Where(q => q.Text.Contains("save"))
				.ToList();
			*/
		}

		#region Related data

		private static void ProjectSamuraisWithQuotes()
		{
			/* var somePropertiesWithQuotes = context.Samurais
				// .Select(s => new { s.Id, s.Name, s.Quotes })
				// .Select(s => new { s.Id, s.Name, s.Quotes.Count })
				.Select(s => new { s.Id, s.Name, HappyQuotes = s.Quotes.Where(q => q.Text.Contains("save") ) })
				.ToList();
			*/

			// Anonymous types are not tracked. Entities that are properties of an anonymous type are tracked.

			var samuraisWithHappyQuotes = context.Samurais
				.Select(s => new {
					Samurai = s,
					HappyQuotes = s.Quotes.Where(q => q.Text.Contains("save"))
				})
				.ToList();

			var firstSamurai = samuraisWithHappyQuotes[0].Samurai.Name += " The Happiest";

			// To see that the first samurai actually has been marked as changed:
			// ChangeTracker: Shift + F9 : "context.ChangeTracker.Entries(), results"
		}

		public struct IdAndName
		{
			public IdAndName(int id, string name)
			{
				this.Id = id;
				this.Name = name;
			}
			public int Id { get; }
			public string Name { get; }
		}

		private static void ProjectSomeProperties()
		{
			// Creates minimal SQL:
			// SELECT [s].[Id], [s].[Name]
			// FROM[Samurais] AS[s]
			// var someProperties = context.Samurais.Select(s => new { s.Id, s.Name }).ToList();
			var someProperties = context.Samurais.Select(s => new IdAndName( s.Id, s.Name )).ToList();
			// .. or cast into a list of dynamic types (Teis question: what does that mean?)
		}

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
