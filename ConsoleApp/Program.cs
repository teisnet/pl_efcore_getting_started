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
			#region Related data

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
			// GetHorseWithSamurai();

			// 6.12 - Working with a relationship that has minimal properties
			// GetSamuraiWithClan();
			GetClanWithSamurais();

			#endregion
		}

		private static void GetClanWithSamurais()
		{
			// The clan has no Samurais property: var clan = context.Clans.Include(c => c.???);
			var clan = context.Clans.Find(1); // clanId
			// Note: The Clan property is populated.
			var samuraisForClan = context.Samurais.Where(s => s.Clan.Id == 1).ToList();
		}

		private static void GetSamuraiWithClan()
		{
			var samurai = context.Samurais.Include(s => s.Clan).FirstOrDefault();
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
	}
}
