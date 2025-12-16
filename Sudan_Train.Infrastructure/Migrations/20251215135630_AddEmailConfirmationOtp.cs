using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trains.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailConfirmationOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailConfirmationOtps",
                schema: "security",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OtpCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfirmationOtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailConfirmationOtps_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationOtp_ExpiresAt",
                schema: "security",
                table: "EmailConfirmationOtps",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationOtp_UserId_Code",
                schema: "security",
                table: "EmailConfirmationOtps",
                columns: new[] { "UserId", "OtpCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailConfirmationOtps",
                schema: "security");
        }
    }
}
