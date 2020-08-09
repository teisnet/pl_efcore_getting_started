using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
	public class BusinessDataLogic
	{
		private SamuraiConsoleContext _context;

		public BusinessDataLogic(SamuraiConsoleContext context)
		{
			this._context = context;
		}

		public BusinessDataLogic()
		{
			_context = new SamuraiConsoleContext();
		}

		public int AddMultipleSamurais(string[] nameList)
		{
			var samuraiList = new List<Samurai>();
			foreach (var name in nameList)
			{
				samuraiList.Add(new Samurai { Name = name });
			}
			_context.Samurais.AddRange(samuraiList);

			var dbResult = _context.SaveChanges();
			return dbResult;
		}

		public int InsertNewSamurai(Samurai samurai)
		{
			_context.Samurais.Add(samurai);
			var dbResult = _context.SaveChanges();
			return dbResult;
		}
	}
}
