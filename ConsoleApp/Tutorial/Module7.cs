using SamuraiApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp.Tutorial
{
	public class Module7
	{
		private static SamuraiContext context = new SamuraiContext();

		public static void Run()
		{
			QuerySamuraiBattleStats();
		}

		private static void QuerySamuraiBattleStats()
		{
			// var stats = context.SamuraiBattleStats.ToList();
			var firstStat = context.SamuraiBattleStats.FirstOrDefault();

			// To see objects that are tracked by the change tracker:
			// QuickWatch: context.ChangeTracker.Entries() > 'Results View'
			// (no objects are tracked because SamuraiBattleStats does not have keys)

			//var sampsonStat = context.SamuraiBattleStats.Where(s => s.Name == "Kambei Shimada").FirstOrDefault();
			// Makes no sense and throws: context.SamuraiBattleStats.Find(22);
		}
	}
}
