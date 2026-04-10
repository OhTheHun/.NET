using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixFieldPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Promotions",
                newName: "DiscountPercent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Promotions",
                newName: "DiscountAmount");
        }
    }
}
