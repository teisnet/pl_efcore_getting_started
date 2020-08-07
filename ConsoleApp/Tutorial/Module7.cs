using Microsoft.EntityFrameworkCore;
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
			// QuerySamuraiBattleStats();
			// QueryUsingRawSql();
			// QueryUsingRawSqlWithInterpolation();
			QueryUsingFromRawSqlStoredProc();
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

		private static void QueryUsingRawSql()
		{
			// 'FromSqlRaw' or 'FromSqlInterpolated' needs to be executed against a defined DbSet.
			
			// 'FromSqlRaw' returns a query so a LINQ execution method is needed.
			// The result will be tracked by EF.
			
			// SQL requirements:
			// 1) Must return data for all properties of the entity type.
			// 2) Coulmn names in the results must match mapped coulmn names (in the entity's table (eg. use clanId and not Clan))
			// 3) Query can't contain related data.
			// 4) Qnly query entities and keyless entities known by the DbContext.

			// var samurais = context.Samurais.FromSqlRaw("Select * from Samurais").ToList();
			var samurais = context.Samurais.FromSqlRaw("Select Id, Name, ClanId from Samurais").Include(s => s.Quotes).ToList();

			// Stored procedures are not 'composable'
		}

		private static void QueryUsingRawSqlWithInterpolation()
		{
			// ALWAYS use 'FromSqlInterpolated' and NOT 'FromSqlRaw' with an interpolated string to avoid SQL injection attacks.
			string name = "Kikuchiyo";
			var samurais = context.Samurais.FromSqlInterpolated($"Select * from Samurais Where Name = {name}").ToList();
		}

		private static void QueryUsingFromRawSqlStoredProc()
		{
			var text = "Save"; // Seems to be case insensitive.
		
			// Both OK.
			// var samurais = context.Samurais.FromSqlRaw("EXEC dbo.SamuraisWhoSaidAWord {0}", text).ToList();
			var samurais = context.Samurais.FromSqlInterpolated($"EXEC dbo.SamuraisWhoSaidAWord {text}").ToList();
			
			// Queries can be extended by LINQ. If EF is not able to sort out the query it will throw at _runtime_!
		}
	}
}
