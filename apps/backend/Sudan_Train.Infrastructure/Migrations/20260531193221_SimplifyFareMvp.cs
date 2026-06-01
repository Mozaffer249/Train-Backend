using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyFareMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookingFee",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "ChildDiscountPercent",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "FuelSurcharge",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "PricePerKm",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "SeniorDiscountPercent",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "PassengerCategory",
                table: "BookingPassengers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BookingFee",
                table: "Fares",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ChildDiscountPercent",
                table: "Fares",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FuelSurcharge",
                table: "Fares",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKm",
                table: "Fares",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SeniorDiscountPercent",
                table: "Fares",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassengerCategory",
                table: "BookingPassengers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
