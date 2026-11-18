using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Uptrail_Backend.Migrations
{
    /// <inheritdoc />
    public partial class addPlatformAdminEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlatformAdmin",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RevokedAt",
                table: "RefreshTokens",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp(3) with time zone",
                oldPrecision: 3);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RefreshTokens",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformAdminId",
                table: "RefreshTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformAdminId",
                table: "Companies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PlatformAdmins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp(3) with time zone", precision: 3, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp(3) with time zone", precision: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformAdmins_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_PlatformAdminId_ExpiresAt",
                table: "RefreshTokens",
                columns: new[] { "PlatformAdminId", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PlatformAdminId",
                table: "Companies",
                column: "PlatformAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAdmins_Email",
                table: "PlatformAdmins",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAdmins_RoleId",
                table: "PlatformAdmins",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_PlatformAdmins_PlatformAdminId",
                table: "Companies",
                column: "PlatformAdminId",
                principalTable: "PlatformAdmins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_PlatformAdmins_PlatformAdminId",
                table: "RefreshTokens",
                column: "PlatformAdminId",
                principalTable: "PlatformAdmins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_PlatformAdmins_PlatformAdminId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_PlatformAdmins_PlatformAdminId",
                table: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "PlatformAdmins");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_PlatformAdminId_ExpiresAt",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Companies_PlatformAdminId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsPlatformAdmin",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "PlatformAdminId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PlatformAdminId",
                table: "Companies");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RevokedAt",
                table: "RefreshTokens",
                type: "timestamp(3) with time zone",
                precision: 3,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp(3) with time zone",
                oldPrecision: 3,
                oldNullable: true);
        }
    }
}
