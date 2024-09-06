using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace M_RENT_Aplikacija.Data.Migrations
{
    /// <inheritdoc />
    public partial class Review : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Recenzije");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Recenzije",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
