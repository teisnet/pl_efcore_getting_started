using Microsoft.EntityFrameworkCore;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace ConsoleApp
{
  internal class Program
  {
    private static SamuraiContext _context = new SamuraiContext();

    private static void Main(string[] args)
    {
      //AddSamurai();
      //GetSamurais("After Add:");
      //InsertMultipleSamurais();
      //GetSamuraisSimpler();
      //QueryFilters();
      //RetrieveAndUpdateSamurai();
      //RetrieveAndUpdateMultipleSamurais();
      //RtrieveAndDeleteASamurai();
      // MultipleDatabaseOperations();
     // InsertBattle();
      QueryAndUpdateBattle_Disconnected();
    }

    private static void InsertMultipleSamurais()
    {
      var samurai = new Samurai { Name = "Sampson" };
      var samurai2 = new Samurai { Name = "Tasha" };
      var samurai3 = new Samurai { Name = "Number3" };
      var samurai4 = new Samurai { Name = "Number 4" };
      _context.Samurais.AddRange(samurai, samurai2, samurai3, samurai4);

      _context.SaveChanges();
    }
    private static void AddSamurai()
    {
      var samurai = new Samurai { Name = "Sampson" };
      _context.Samurais.Add(samurai);
      _context.SaveChanges();
    }
    private static void GetSamuraisSimpler()
    {
      //var samurais = context.Samurais.ToList();
      var query = _context.Samurais;
      //var samurais = query.ToList();
      foreach (var samurai in query)
      {
        Console.WriteLine(samurai.Name);
      }
    }
    private static void GetSamurais(string text)
    {
      var samurais = _context.Samurais.ToList();
      Console.WriteLine($"{text}: Samurai count is {samurais.Count}");
      //foreach (var samurai in samurais)
      //{
      //  Console.WriteLine(samurai.Name);
      //}
    }
    private static void QueryFilters()
    {
      var name = "Sampson";
      var samurais = _context.Samurais.Where(s => s.Name == "Sampson").ToList();
      //var samurai = _context.Samurais.Find(2);
      //var filter = "J%";
      //var samurais=_context.Samurais.Where(s=>EF.Functions.Like(s.Name,filter)).ToList();
      //var last = _context.Samurais.OrderBy(s => s.Id).LastOrDefault(s => s.Name == name);
      //the following will throw an exception:
      //var lastNoOrder= _context.Samurais.LastOrDefault(s => s.Name == name);
    }

    private static void RetrieveAndUpdateSamurai()
    {
      var samurai = _context.Samurais.FirstOrDefault();
      samurai.Name += "San";
      _context.SaveChanges();
    }
    private static void RetrieveAndUpdateMultipleSamurais()
    {
      var samurais = _context.Samurais.Skip(1).Take(4).ToList();
      samurais.ForEach(s => s.Name += "San");
      _context.SaveChanges();
    }
    private static void RetrieveAndDeleteASamurai()
    {
      var samurai = _context.Samurais.Find(18);
      _context.Samurais.Remove(samurai);
      _context.SaveChanges();
    }
    private static void MultipleDatabaseOperations()
    {
      var samurai = _context.Samurais.FirstOrDefault();
      samurai.Name += "San";
      _context.Samurais.Add(new Samurai { Name = "Kikuchiyo" });
      _context.SaveChanges();
    }

    private static void InsertBattle()
    {
      _context.Battles.Add(new Battle
      {
        Name = "Battle of Okehazama",
        StartDate = new DateTime(1560, 05, 01),
        EndDate = new DateTime(1560, 06, 15)
      });
      _context.SaveChanges();
    }

    private static void QueryAndUpdateBattle_Disconnected()
    {
      var battle = _context.Battles.AsNoTracking().FirstOrDefault();
      battle.EndDate = new DateTime(1560, 06, 30);
      using (var newContextInstance = new SamuraiContext())
      {
        newContextInstance.Battles.Update(battle);
        newContextInstance.SaveChanges();
      }
    }
  }
}