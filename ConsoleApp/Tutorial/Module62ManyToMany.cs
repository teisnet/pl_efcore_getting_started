using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp.Tutorial
{
	public class Module62ManyToMany
	{
		/* Module 6 - Interact With Related Data
		*  Second part: Many-to-many relationships (Samurai-Battles)
		*/

		private static SamuraiConsoleContext context = new SamuraiConsoleContext();

		public static void Run()
		{
			// 6.8 - Create and change many-to-many relationships
			// JoinBattleAndSamurai();
			// EnlistSamuraiIntoABattle();
			// RemoveJoinBetweenSamuraiAndBattleSimple();

			// 6.9 - Query across many-to-many relationships
			// GetSamuraiWithBattles();
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
					Battles = s.SamuraiBattles.Select(sb => sb.Battle)
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
	}
}
