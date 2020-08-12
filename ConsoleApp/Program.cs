using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
  internal class Program
  {
    private static SamuraiContext _context = new SamuraiContext();

    private static void Main(string[] args)
    {
      //QuerySamuraiBattleStats();
      //QueryUsingRawSql();
      //QueryUsingRawSqlStoredProcParameters();
      //InterpolatedQueryUsingRawSqlStoredProcParameters();
      ExecuteSomeRawSql();
     }

    private static void QuerySamuraiBattleStats()
    {
      var stats = _context.SamuraiBattleStats.ToList();
      var firstStat = _context.SamuraiBattleStats.FirstOrDefault();
      var sampsonStat = _context.SamuraiBattleStats.Where(s => s.Name == "SampsonSan").FirstOrDefault();

      _context.SamuraiBattleStats.Find(1);
    } 
    private static void QueryUsingRawSql()
    {
      var samurais = _context.Samurais.FromSqlRaw(
        "Select Id, Name, ClanId from Samurais").Include(s=>s.Quotes).ToList();
    }
    private static void QueryUsingRawSqlStoredProcParameters()
    {
      var text = "Happy";
      var samurais = _context.Samurais.FromSqlRaw(
        "EXEC dbo.SamuraisWithFilteredQuotes {0} ", text).ToList();
    }
    private static void InterpolatedQueryUsingRawSqlStoredProcParameters()
    {
      var text = "Happy";
      var samurais = _context.Samurais.FromSqlInterpolated(
        $"EXEC dbo.SamuraisWithFilteredQuotes {text}").ToList();
    }

   private static void ExecuteSomeRawSql()
    {
      var samuraiId = 22;
      //var x =_context.Database
      //         .ExecuteSqlRaw("EXEC DeleteQuotesForSamurai {0}", samuraiId );
      samuraiId = 31;
       _context.Database
             .ExecuteSqlInterpolated($"EXEC DeleteQuotesForSamurai {samuraiId}");

    }

  }
}