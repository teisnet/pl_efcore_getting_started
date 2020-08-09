using ConsoleApp.Tutorial;
using System;

namespace ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			// Module2FirstApp.Run();
			// Module5Simple.Run();
			// Module61OneToMany.Run();
			// Module62ManyToMany.Run();
			// Module63OneToOne.Run();
			// Module7.Run();

			// Module 9 - Testing
			InsertMultipleSamurais();
		}

		private static void InsertMultipleSamurais()
		{
			var _bizdata = new BusinessDataLogic();
			var samuraiNames = new string[] { "Sampson", "Tasha", "Number3", "Number4" };
			var newSamuraisCreated = _bizdata.AddMultipleSamurais(samuraiNames);
		}
	}
}
