using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffStationsAndOpsEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: convert the existing string Status values to their numeric
            // enum representations while still in nvarchar form, so the
            // subsequent ALTER COLUMN int can cast them safely.
            //
            // TripStatus: Scheduled=0, Departed=1, Arrived=2, Cancelled=3, Delayed=4
            // Legacy values: "In Transit" → Departed (1), "Completed" → Arrived (2).
            migrationBuilder.Sql(@"
                UPDATE Trip SET Status = CASE Status
                    WHEN 'Scheduled'  THEN '0'
                    WHEN 'Departed'   THEN '1'
                    WHEN 'In Transit' THEN '1'
                    WHEN 'Arrived'    THEN '2'
                    WHEN 'Completed'  THEN '2'
                    WHEN 'Cancelled'  THEN '3'
                    WHEN 'Delayed'    THEN '4'
                    ELSE '0'
                END;");

            // TicketStatus: Issued=0, Boarded=1, NoShow=2, Cancelled=3
            migrationBuilder.Sql(@"
                UPDATE Tickets SET Status = CASE Status
                    WHEN 'Issued'    THEN '0'
                    WHEN 'Boarded'   THEN '1'
                    WHEN 'NoShow'    THEN '2'
                    WHEN 'Cancelled' THEN '3'
                    ELSE '0'
                END;");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Scheduled");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Issued");

            migrationBuilder.AddColumn<DateTime>(
                name: "BoardedAt",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoardedByUserId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffStations_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffStations_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BoardedByUserId",
                table: "Tickets",
                column: "BoardedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffStations_StationId",
                table: "StaffStations",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffStations_UserId_StationId",
                table: "StaffStations",
                columns: new[] { "UserId", "StationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_BoardedByUserId",
                table: "Tickets",
                column: "BoardedByUserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_BoardedByUserId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "StaffStations");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_BoardedByUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BoardedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BoardedByUserId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Trip",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Scheduled",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Issued",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
