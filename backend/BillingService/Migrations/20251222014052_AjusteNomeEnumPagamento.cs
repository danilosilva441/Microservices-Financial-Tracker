using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Migrations
{
    /// <inheritdoc />
    public partial class AjusteNomeEnumPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetodoPagamentoId",
                table: "FaturamentosParciais");

            // 2. Cria a coluna nova (Inteiro) limpa
            migrationBuilder.AddColumn<int>(
                name: "MetodoPagamentoId",
                table: "FaturamentosParciais",
                type: "integer",
                nullable: false,
                defaultValue: 0); // ou 1 (Dinheiro) se preferir
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "MetodoPagamentoId",
                table: "FaturamentosParciais",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "MetodoPagamento",
                table: "FaturamentosParciais",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
