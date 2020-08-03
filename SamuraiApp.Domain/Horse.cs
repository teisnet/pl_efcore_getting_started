namespace SamuraiApp.Domain
{
	public class Horse
	{
		// Question: How to make sure that there are only one horse points to the same samurai (only allowing unique values for the field)
		// Answer: EF automatically creates a unique index for the SamuraiId field.
		public int Id { get; set; }
		public string Name { get; set; }

		// To make a one-to-one relationship, the foreign SamuraiId key
		// along with the 'Horse' navigation property in the Samurai class is enough.
		// For a one-to-one relationship, it would be possible to make EF Core merge the into the same table.
		// Since the int cannot be null, the table field is set to 'NOT NULL'.
		// Thus a horse cannot exist without a samurai.
		public int SamuraiId { get; set; }
	}
}
