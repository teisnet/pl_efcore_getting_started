using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp;
using System.Linq;
using System.Threading;

namespace Tests
{
	[TestClass]
	public class BizDataLogicTests
	{
		[TestMethod]
		public void AddMultipleSamuraisReturnsCorrectNumberOfInsertedRows()
		{
			var builder = new DbContextOptionsBuilder();
			builder.UseInMemoryDatabase("AddMultipleSamurais");

			using (var context = new SamuraiConsoleContext(builder.Options))
			{
				var bizLogic = new BusinessDataLogic(context);
				var nameList = new string[] { "Kikuchiyo", "Kyuzo", "Rikchi"};
				var result = bizLogic.AddMultipleSamurais(nameList);
				Assert.AreEqual(nameList.Count(), result);
			}
		}

		[TestMethod]
		public void CanInsertSingeSamurai()
		{
			var builder = new DbContextOptionsBuilder();
			builder.UseInMemoryDatabase("InsertSingeSamurai");

			using (var context = new SamuraiConsoleContext(builder.Options))
			{
				var bizLogic = new BusinessDataLogic(context);
				bizLogic.InsertNewSamurai(new Samurai());
			}

			using (var context2 = new SamuraiConsoleContext(builder.Options))
			{
				Assert.AreEqual (1, context2.Samurais.Count());
			}
		}

		[TestMethod]
		public void CanInsertSamuraiWithQuotes()
		{
			var samuraiGraph = new Samurai
			{
				Name = "Kyūzō",
				Quotes = new List<Quote>
				{
					new Quote { Text = "Watch out for my sharp sword" },
					new Quote { Text = "I told you to watch out for my sharp sword! Oh well!" }
				}
			};

			var builder = new DbContextOptionsBuilder();
			builder.UseInMemoryDatabase("SamuraiWithQuotes");
			
			using (var context = new SamuraiConsoleContext(builder.Options))
			{
				var bizlogic = new BusinessDataLogic(context);
				var result = bizlogic.InsertNewSamurai(samuraiGraph);
			}

			using (var context2 = new SamuraiConsoleContext(builder.Options))
			{
				var samurai = context2.Samurais.Include(s => s.Quotes).FirstOrDefaultAsync().Result;
				Assert.AreEqual(2, samurai.Quotes.Count);
				// Check if the InMemory database is shared across tests (it is): var samurais = context2.Samurais.ToList();
			}
		}

		// Teis question: What does the 'TestCategory' do?
		[TestMethod, TestCategory("SamuraiWithQuotes")]
		public void CanGetSamuraiWithQuotes()
		{
			int samuraiId;
			var builder = new DbContextOptionsBuilder();
			// Teis question: won't the reuse of the InMemory db name 'SamuraiWithQuotes'
			// interfere with the previous test using the same name?
			// Answer: The database is shared across tests but they don't interefere with each other (bad practice though)
			builder.UseInMemoryDatabase("SamuraiWithQuotes");

			using (var seedContext = new SamuraiConsoleContext(builder.Options))
			{
				var samuraiGraph = new Samurai
				{
					Name = "Kyūzō",
					Quotes = new List<Quote> {
						new Quote { Text = "Watch out for my sharp sword" },
						new Quote { Text = "I told you to watch out for my sharp sword! Oh well!" }
					}
				};

				seedContext.Samurais.Add(samuraiGraph);
				seedContext.SaveChanges();
				samuraiId = samuraiGraph.Id;
			}

			using (var context = new SamuraiConsoleContext(builder.Options))
			{
				// Check if the InMemory database is shared across tests (it is): var samurais = context.Samurais.ToList();
				var bizlogic = new BusinessDataLogic(context);
				var result = bizlogic.GetSamuraiWithQuotes(samuraiId);
				Assert.AreEqual(2, result.Quotes.Count);
			}
		}
	}
}
