using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp.Tutorial
{
	public class Module61OneToMany
	{
		/* Module 6 - Interact With Related Data
		*  First part: One-to-many relationships (Samurai-Quotes)
		*/

		private static SamuraiContext context = new SamuraiContext();

		public static void Run()
		{
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
			var someProperties = context.Samurais.Select(s => new IdAndName(s.Id, s.Name)).ToList();
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
	}
}
