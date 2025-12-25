using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBoundaryFieldsToGeography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ServiceRadiusKm",
                table: "Stations",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StationType",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoundaryPolygon",
                table: "States",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxEast",
                table: "States",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxNorth",
                table: "States",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxSouth",
                table: "States",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxWest",
                table: "States",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoundaryPolygon",
                table: "Regions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxEast",
                table: "Regions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxNorth",
                table: "Regions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxSouth",
                table: "Regions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxWest",
                table: "Regions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoundaryPolygon",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxEast",
                table: "Cities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxNorth",
                table: "Cities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxSouth",
                table: "Cities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoundingBoxWest",
                table: "Cities",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceRadiusKm",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "StationType",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "BoundaryPolygon",
                table: "States");

            migrationBuilder.DropColumn(
                name: "BoundingBoxEast",
                table: "States");

            migrationBuilder.DropColumn(
                name: "BoundingBoxNorth",
                table: "States");

            migrationBuilder.DropColumn(
                name: "BoundingBoxSouth",
                table: "States");

            migrationBuilder.DropColumn(
                name: "BoundingBoxWest",
                table: "States");

            migrationBuilder.DropColumn(
                name: "BoundaryPolygon",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "BoundingBoxEast",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "BoundingBoxNorth",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "BoundingBoxSouth",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "BoundingBoxWest",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "BoundaryPolygon",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "BoundingBoxEast",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "BoundingBoxNorth",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "BoundingBoxSouth",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "BoundingBoxWest",
                table: "Cities");
        }
    }
}
