using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace ConsoleApp.Tutorial
{
	public class Module2FirstApp
	{
		private static SamuraiConsoleContext context = new SamuraiConsoleContext();

		public static void Run()
		{
			context.Database.EnsureCreated();

			GetSamurais("Before add");
			AddSamurai();
			GetSamurais("After add");
		}

		private static void AddSamurai()
		{
			var samurai = new Samurai { Name = "Julie" };
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
	}
}
