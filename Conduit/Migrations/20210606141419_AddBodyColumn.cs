using Microsoft.EntityFrameworkCore.Migrations;

namespace Conduit.Migrations
{
    public partial class AddBodyColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Article",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1200)",
                oldMaxLength: 1200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Article",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "Article");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Article",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
