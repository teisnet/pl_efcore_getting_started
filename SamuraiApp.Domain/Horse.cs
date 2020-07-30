namespace SamuraiApp.Domain
{
	public class Horse
	{
		public int Id { get; set; }
		public string Name { get; set; }

		// To make a one-to-one relationship, the foreign SamuraiId key
		// along with the 'Horse' navigation property in the Samurai class is enough.
		// For a one-to-one relationship, it would be possible to make EF Core merge the into the same table.
		public int SamuraiId { get; set; }
	}
}
