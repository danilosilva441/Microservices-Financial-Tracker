using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class V2_Schema_Seguro_Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeDaEmpresa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StatusDaSubscricao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DataDeCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    ReportsToUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_ReportsToUserId",
                        column: x => x.ReportsToUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Name", "NormalizedName", "TenantId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-a1a1-a1a1a1a1a1a1"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8645), false, "User", "USER", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8650) },
                    { new Guid("a7b8c9d0-e1f2-34a5-a7a7-a7a7a7a7a7a7"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8810), false, "Operador", "OPERADOR", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8811) },
                    { new Guid("b2c3d4e5-f6a7-890b-b2b2-b2b2b2b2b2b2"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8767), false, "Admin", "ADMIN", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8768) },
                    { new Guid("c3d4e5f6-a7b8-90c1-c3c3-c3c3c3c3c3c3"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8774), false, "Dev", "DEV", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8774) },
                    { new Guid("d4e5f6a7-b8c9-01d2-d4d4-d4d4d4d4d4d4"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8778), false, "Gerente", "GERENTE", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8779) },
                    { new Guid("e5f6a7b8-c9d0-12e3-e5e5-e5e5e5e5e5e5"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8783), false, "Supervisor", "SUPERVISOR", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8783) },
                    { new Guid("f6a7b8c9-d0e1-23f4-f6f6-f6f6f6f6f6f6"), new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8806), false, "Lider", "LIDER", null, new DateTime(2025, 11, 15, 0, 14, 42, 982, DateTimeKind.Utc).AddTicks(8806) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UsersId",
                table: "UserRoles",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ReportsToUserId",
                table: "Users",
                column: "ReportsToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
