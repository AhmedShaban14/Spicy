using Microsoft.EntityFrameworkCore.Migrations;

namespace Spicy.Data.Migrations
{
    public partial class addCouponsCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    CouponType = table.Column<string>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    MinimalAmount = table.Column<decimal>(nullable: false),
                    IsActice = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");
        }
    }
}
