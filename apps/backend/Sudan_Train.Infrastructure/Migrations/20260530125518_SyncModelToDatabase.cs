using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelToDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Governorates_GovernorateId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "Governorates");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropIndex(
                name: "IX_Cities_GovernorateId_NameEn",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Fares",
                newName: "BasePrice");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Stations",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Stations",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceNote",
                table: "Stations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Routes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceNote",
                table: "Routes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "Fares",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0.15m,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AddColumn<int>(
                name: "DestinationStationId",
                table: "Fares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginStationId",
                table: "Fares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKm",
                table: "Fares",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Fares",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Cities",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Cities",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Fares_DestinationStationId",
                table: "Fares",
                column: "DestinationStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fares_OriginStationId",
                table: "Fares",
                column: "OriginStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fares_RouteId",
                table: "Fares",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fares_Routes_RouteId",
                table: "Fares",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Fares_Stations_DestinationStationId",
                table: "Fares",
                column: "DestinationStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fares_Stations_OriginStationId",
                table: "Fares",
                column: "OriginStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fares_Routes_RouteId",
                table: "Fares");

            migrationBuilder.DropForeignKey(
                name: "FK_Fares_Stations_DestinationStationId",
                table: "Fares");

            migrationBuilder.DropForeignKey(
                name: "FK_Fares_Stations_OriginStationId",
                table: "Fares");

            migrationBuilder.DropIndex(
                name: "IX_Fares_DestinationStationId",
                table: "Fares");

            migrationBuilder.DropIndex(
                name: "IX_Fares_OriginStationId",
                table: "Fares");

            migrationBuilder.DropIndex(
                name: "IX_Fares_RouteId",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "MaintenanceNote",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "MaintenanceNote",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "DestinationStationId",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "OriginStationId",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "PricePerKm",
                table: "Fares");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Fares");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "Fares",
                newName: "Price");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Stations",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Stations",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatRate",
                table: "Fares",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldDefaultValue: 0.15m);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Cities",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Cities",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "Cities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoundaryPolygon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoundingBoxEast = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxNorth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxSouth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxWest = table.Column<double>(type: "float", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FormattedAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GoogleSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFromGoogle = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Governorates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    BoundaryPolygon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoundingBoxEast = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxNorth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxSouth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxWest = table.Column<double>(type: "float", nullable: true),
                    FormattedAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GoogleSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFromGoogle = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PlusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Governorates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Governorates_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_GovernorateId_NameEn",
                table: "Cities",
                columns: new[] { "GovernorateId", "NameEn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Areas_Code",
                table: "Areas",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Governorates_AreaId",
                table: "Governorates",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Governorates_GovernorateId",
                table: "Cities",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
