using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOperacaoAndFaturamentoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Operacoes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Operacoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Moeda",
                table: "Operacoes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Operacoes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Moeda",
                table: "Faturamentos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "Moeda",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "Moeda",
                table: "Faturamentos");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Operacoes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
