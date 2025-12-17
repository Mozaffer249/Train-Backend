using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIpLoginAttemptTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IpLoginAttempts",
                schema: "security",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WasSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpLoginAttempts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IpLoginAttempts_AttemptTime",
                schema: "security",
                table: "IpLoginAttempts",
                column: "AttemptTime");

            migrationBuilder.CreateIndex(
                name: "IX_IpLoginAttempts_IpAddress_AttemptTime",
                schema: "security",
                table: "IpLoginAttempts",
                columns: new[] { "IpAddress", "AttemptTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IpLoginAttempts",
                schema: "security");
        }
    }
}
