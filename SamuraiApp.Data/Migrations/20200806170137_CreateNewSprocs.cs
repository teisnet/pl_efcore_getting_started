using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiApp.Data.Migrations
{
	public partial class CreateNewSprocs : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(
				// Select samurais that have said a specific word/phrase in their quotes.
				// Teis: Will this return duplicate rows if the word is contained
				// in more that one of the samurai's quotes, due to the inner join.
				@"CREATE PROCEDURE dbo.SamuraisWhoSaidAWord @text VARCHAR(20)
					AS
					SELECT Samurais.Id, Samurais.Name, Samurais.ClanId
					FROM Samurais INNER JOIN
					Quotes ON Samurais.Id = Quotes.SamuraiId
					WHERE (Quotes.Text LIKE '%'+@text+'%')");

			migrationBuilder.Sql(
				// Deletes a specific samurai by its id.
				@"CREATE PROCEDURE dbo.DeleteQuotesFromSamurai @samuraiId int
					AS
					DELETE FROM Quotes
					WHERE Quotes.SamuraiId=@samuraiId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"DROP PROCEDURE dbo.SamuraisWhoSaidAWord");
			migrationBuilder.Sql(@"DROP PROCEDURE dbo.DeleteQuotesFromSamurai");
		}
	}
}
