using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M_RENT_Aplikacija.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManualChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Automobili",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Automobili");
        }
    }
}
