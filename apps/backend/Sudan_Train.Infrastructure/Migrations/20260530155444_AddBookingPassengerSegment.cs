using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingPassengerSegment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlightingStationId",
                table: "BookingPassengers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BoardingStationId",
                table: "BookingPassengers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_AlightingStationId",
                table: "BookingPassengers",
                column: "AlightingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_BoardingStationId",
                table: "BookingPassengers",
                column: "BoardingStationId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_TripId_TripSeatId",
                table: "BookingPassengers",
                columns: new[] { "TripId", "TripSeatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPassengers_Stations_AlightingStationId",
                table: "BookingPassengers",
                column: "AlightingStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPassengers_Stations_BoardingStationId",
                table: "BookingPassengers",
                column: "BoardingStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPassengers_Stations_AlightingStationId",
                table: "BookingPassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingPassengers_Stations_BoardingStationId",
                table: "BookingPassengers");

            migrationBuilder.DropIndex(
                name: "IX_BookingPassengers_AlightingStationId",
                table: "BookingPassengers");

            migrationBuilder.DropIndex(
                name: "IX_BookingPassengers_BoardingStationId",
                table: "BookingPassengers");

            migrationBuilder.DropIndex(
                name: "IX_BookingPassengers_TripId_TripSeatId",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "AlightingStationId",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "BoardingStationId",
                table: "BookingPassengers");
        }
    }
}
