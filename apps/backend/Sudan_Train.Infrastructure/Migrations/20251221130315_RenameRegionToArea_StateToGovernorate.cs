using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameRegionToArea_StateToGovernorate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_States_StateId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Regions");

            migrationBuilder.RenameColumn(
                name: "StateId",
                table: "Cities",
                newName: "GovernorateId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_StateId_NameEn",
                table: "Cities",
                newName: "IX_Cities_GovernorateId_NameEn");

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    FormattedAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GoogleSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFromGoogle = table.Column<bool>(type: "bit", nullable: false),
                    BoundaryPolygon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoundingBoxNorth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxSouth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxEast = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxWest = table.Column<double>(type: "float", nullable: true)
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
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    GooglePlaceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    FormattedAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GoogleSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFromGoogle = table.Column<bool>(type: "bit", nullable: false),
                    BoundaryPolygon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoundingBoxNorth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxSouth = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxEast = table.Column<double>(type: "float", nullable: true),
                    BoundingBoxWest = table.Column<double>(type: "float", nullable: true)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Governorates_GovernorateId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "Governorates");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.RenameColumn(
                name: "GovernorateId",
                table: "Cities",
                newName: "StateId");

            migrationBuilder.RenameIndex(
                name: "IX_Cities_GovernorateId_NameEn",
                table: "Cities",
                newName: "IX_Cities_StateId_NameEn");

            migrationBuilder.CreateTable(
                name: "Regions",
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
                    table.PrimaryKey("PK_Regions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Regions_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Code",
                table: "Regions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_NameEn",
                table: "States",
                column: "NameEn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_RegionId",
                table: "States",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_States_StateId",
                table: "Cities",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
