using Microsoft.EntityFrameworkCore.Migrations;

namespace Spicy.Data.Migrations
{
    public partial class AddMenuItemsColforce : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpicyTypes",
                table: "MenuItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpicyTypes",
                table: "MenuItems");
        }
    }
}
