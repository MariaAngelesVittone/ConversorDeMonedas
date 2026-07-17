using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyLeyendSnapshotToHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromCurrencyLeyend",
                table: "ConversionHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ToCurrencyLeyend",
                table: "ConversionHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromCurrencyLeyend",
                table: "ConversionHistories");

            migrationBuilder.DropColumn(
                name: "ToCurrencyLeyend",
                table: "ConversionHistories");
        }
    }
}
