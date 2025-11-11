using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class V2_Auth_Tenant_Null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Tenants",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Roles",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7300), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7303) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7365), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7366) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7322), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7323) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7328), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7328) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7332), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7333) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7337), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7338) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7360), null, new DateTime(2025, 11, 9, 20, 36, 3, 678, DateTimeKind.Utc).AddTicks(7361) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Tenants",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4061), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4064) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4121), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4121) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4080), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4081) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4102), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4103) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4107), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4108) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4112), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4112) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"),
                columns: new[] { "CreatedAt", "TenantId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4116), new Guid("00000000-0000-0000-0000-000000000000"), new DateTime(2025, 11, 9, 19, 4, 33, 994, DateTimeKind.Utc).AddTicks(4117) });
        }
    }
}
