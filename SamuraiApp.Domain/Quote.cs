namespace SamuraiApp.Domain
{
	/*
	 * 'Quote' has a many-to-one relationship with 'Samurai'
	 */

	public class Quote
	{
		public int Id { get; set; }
		public string Text { get; set; }
		public Samurai Samurai { get; set; }
		public int SamuraiId { get; set; }
	}
}