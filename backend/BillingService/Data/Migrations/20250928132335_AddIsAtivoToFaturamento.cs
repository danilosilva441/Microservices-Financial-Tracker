using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAtivoToFaturamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAtivo",
                table: "Faturamentos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAtivo",
                table: "Faturamentos");
        }
    }
}
