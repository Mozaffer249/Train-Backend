using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatHolds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeatHolds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoldGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    TripSeatId = table.Column<int>(type: "int", nullable: false),
                    BoardingStationId = table.Column<int>(type: "int", nullable: false),
                    AlightingStationId = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatHolds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeatHolds_TripSeats_TripSeatId",
                        column: x => x.TripSeatId,
                        principalTable: "TripSeats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeatHolds_TripId_ExpiresAt",
                table: "SeatHolds",
                columns: new[] { "TripId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SeatHolds_TripSeatId",
                table: "SeatHolds",
                column: "TripSeatId");

            migrationBuilder.CreateIndex(
                name: "IX_SeatHolds_UserId_HoldGroupId",
                table: "SeatHolds",
                columns: new[] { "UserId", "HoldGroupId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatHolds");
        }
    }
}
