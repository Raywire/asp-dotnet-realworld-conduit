using Microsoft.EntityFrameworkCore.Migrations;

namespace Conduit.Migrations
{
    public partial class RemoveFavoritedAndFavoritesCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorited",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "FavoritesCount",
                table: "Article");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorited",
                table: "Article",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FavoritesCount",
                table: "Article",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
