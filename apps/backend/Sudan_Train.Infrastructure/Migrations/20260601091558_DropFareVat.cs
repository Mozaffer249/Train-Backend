using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropFareVat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "Fares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VatRate",
                table: "Fares",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0.15m);
        }
    }
}
