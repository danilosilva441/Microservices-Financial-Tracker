using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Migrations
{
    /// <inheritdoc />
    public partial class AlterarMetodoPagamentoParaEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FaturamentosParciais_MetodosPagamento_MetodoPagamentoId",
                table: "FaturamentosParciais");

            migrationBuilder.DropIndex(
                name: "IX_FaturamentosParciais_MetodoPagamentoId",
                table: "FaturamentosParciais");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "WorkSchedules",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "ShiftBreaks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "MetodoPagamento",
                table: "FaturamentosParciais",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "EmployeeShifts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetodoPagamento",
                table: "FaturamentosParciais");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "WorkSchedules",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "ShiftBreaks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "EmployeeShifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentosParciais_MetodoPagamentoId",
                table: "FaturamentosParciais",
                column: "MetodoPagamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_FaturamentosParciais_MetodosPagamento_MetodoPagamentoId",
                table: "FaturamentosParciais",
                column: "MetodoPagamentoId",
                principalTable: "MetodosPagamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
