using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoogleIntegrationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessStatus",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "Stations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Stations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoogleSyncedAt",
                table: "Stations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleType",
                table: "Stations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromGoogle",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PlusCode",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "States",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "States",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoogleSyncedAt",
                table: "States",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromGoogle",
                table: "States",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "States",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "States",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlusCode",
                table: "States",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "Regions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Regions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoogleSyncedAt",
                table: "Regions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromGoogle",
                table: "Regions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Regions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Regions",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlusCode",
                table: "Regions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormattedAddress",
                table: "Cities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Cities",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoogleSyncedAt",
                table: "Cities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromGoogle",
                table: "Cities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Cities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Cities",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlusCode",
                table: "Cities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessStatus",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "GoogleSyncedAt",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "GoogleType",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "IsFromGoogle",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "PlusCode",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "States");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "States");

            migrationBuilder.DropColumn(
                name: "GoogleSyncedAt",
                table: "States");

            migrationBuilder.DropColumn(
                name: "IsFromGoogle",
                table: "States");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "States");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "States");

            migrationBuilder.DropColumn(
                name: "PlusCode",
                table: "States");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "GoogleSyncedAt",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "IsFromGoogle",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "PlusCode",
                table: "Regions");

            migrationBuilder.DropColumn(
                name: "FormattedAddress",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "GoogleSyncedAt",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "IsFromGoogle",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "PlusCode",
                table: "Cities");
        }
    }
}
