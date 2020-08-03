using System;
using System.Collections.Generic;
using System.Text;

namespace SamuraiApp.Domain
{
	public class Samurai
	{
		public Samurai()
		{
			Quotes = new List<Quote>();
			SamuraiBattles = new List<SamuraiBattle>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public List<Quote> Quotes { get; set; }

		// ClanId is left out make turorial challenge harder (including list of samurais in Clan).
		public Clan Clan { get; set; }
		public List<SamuraiBattle> SamuraiBattles { get; set; }

		// The Horse relationship
		// To make a one-to-one relationship, the 'Horse' navigation property
		// along with the foreign key in the Horse class is enough.

		// The dependent end, the Horse, is always optional. The Samurai don't need to have a Horse.
		// There is no way to apply that constraint in the model or in the database. It can only be done in the businiss logic.
		public Horse Horse { get; set; }
	}
}
