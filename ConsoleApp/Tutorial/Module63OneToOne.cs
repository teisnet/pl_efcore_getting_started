﻿using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp.Tutorial
{
	public class Module63OneToOne
	{
		/* Module 6 - Interact With Related Data
		*  Third part: One-to-one relationships (Samurai-Horse)
		*  Also: Relationship with minimal properties (Samurai-clan)
		*/

		private static SamuraiContext context = new SamuraiContext();

		public void Run()
		{
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
			var samurai = context.Samurais.AsNoTracking().FirstOrDefault(s => s.Id == 18);
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
	}
}
