using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMoedaColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moeda",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "Moeda",
                table: "Faturamentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Moeda",
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
    }
}
