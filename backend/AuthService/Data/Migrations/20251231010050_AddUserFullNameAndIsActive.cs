using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFullNameAndIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(3972), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(3975) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4074), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4075) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4042), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4042) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4049), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4050) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4055), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4056) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4062), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4062) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4069), new DateTime(2025, 12, 31, 1, 0, 48, 294, DateTimeKind.Utc).AddTicks(4069) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8645), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8650) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8810), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8811) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8767), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8768) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8774), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8774) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8778), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8779) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8783), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8783) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8806), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8806) });
        }
    }
}
