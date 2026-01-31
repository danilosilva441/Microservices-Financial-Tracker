using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFechamentoCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConferidoPorUserId",
                table: "FaturamentosDiarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataConferencia",
                table: "FaturamentosDiarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFechamento",
                table: "FaturamentosDiarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Diferenca",
                table: "FaturamentosDiarios",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FechadoPorUserId",
                table: "FaturamentosDiarios",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashAssinatura",
                table: "FaturamentosDiarios",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacoesConferencia",
                table: "FaturamentosDiarios",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCaixa",
                table: "FaturamentosDiarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorConferido",
                table: "FaturamentosDiarios",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotalCalculado",
                table: "FaturamentosDiarios",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConferidoPorUserId",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "DataConferencia",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "DataFechamento",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "Diferenca",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "FechadoPorUserId",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "HashAssinatura",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "ObservacoesConferencia",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "StatusCaixa",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "ValorConferido",
                table: "FaturamentosDiarios");

            migrationBuilder.DropColumn(
                name: "ValorTotalCalculado",
                table: "FaturamentosDiarios");
        }
    }
}
