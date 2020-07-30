namespace SamuraiApp.Domain
{
	public class Horse
	{
		public int Id { get; set; }
		public string Name { get; set; }

		// To make a one-to-one relationship, the foreign SamuraiId key
		// along with the 'Horse' navigation property in the Samurai class is enough.
		public int SamuraiId { get; set; }
	}
}
