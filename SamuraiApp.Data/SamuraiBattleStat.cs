using System;
using System.Collections.Generic;
using System.Text;

namespace SamuraiApp.Domain
{
	public class SamuraiBattleStat
	{
		// Notice: No key property!
		public string Name { get; set; }
		public int? NumberOfBattles { get; set; }
		public string EarliestBattle { get; set; }
	}
}
