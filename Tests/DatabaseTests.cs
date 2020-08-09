using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tests
{
	[TestClass]
	public class DatabaseTests
	{
		[TestMethod]
		public void CanInsertSamuraiIntoDatabase()
		{
			using (var context = new SamuraiConsoleContext())
			{
				// context.Database.EnsureDeleted(); // Takes long time
				context.Database.EnsureCreated();
				var samurai = new Samurai();
				// Properties are non-nullable needs to be populated in advance, eg. the 'Name' field.
				context.Samurais.Add(samurai);
				Debug.WriteLine($"Before save: {samurai.Id}");

				context.SaveChanges();
				Debug.WriteLine($"After save: {samurai.Id}");

				Assert.AreNotEqual(0, samurai.Id);
			}
		}
	}
}
