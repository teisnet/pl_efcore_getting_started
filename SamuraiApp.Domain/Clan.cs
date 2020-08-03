namespace SamuraiApp.Domain
{
	/*
	 * 'Clan' has a one-to-many relationship with 'Samurai'
	 */

	public class Clan
	{
		public int Id { get; set; }
		public string ClanName { get; set; }

		// List of samurais is left out make turorial challenge harder (including a clanId in samurai).
	}
}