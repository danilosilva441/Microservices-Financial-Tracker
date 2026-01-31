using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddValorTotalToFaturamentoDiario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "FaturamentosDiarios",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "FaturamentosDiarios");
        }
    }
}
