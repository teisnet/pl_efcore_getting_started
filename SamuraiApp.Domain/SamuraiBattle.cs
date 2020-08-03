using System;
using System.Collections.Generic;
using System.Text;

namespace SamuraiApp.Domain
{
	/*
	 * 'SamuraiBattle' is the joint table for the
	 * many-to-many relationship between 'Samurai' and 'Battle'.
	 */

	public class SamuraiBattle
	{
		// The key values are required
		public int SamuraiId { get; set; }
		public int BattleId { get; set; }

		// The navigation properties are optional
		public Samurai Samurai { get; set; }
		public Battle Battle { get; set; }
	}
}
