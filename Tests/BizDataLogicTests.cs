using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp;
using System.Linq;

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
	}
}
