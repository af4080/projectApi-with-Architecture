using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace projectApiAngular.Migrations
{
    /// <inheritdoc />
    public partial class NameGiftIsUniq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Gifts_Name",
                table: "Gifts",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Gifts_Name",
                table: "Gifts");
        }
    }
}
