using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Migrations
{
    /// <inheritdoc />
    public partial class Meta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Metas",
                newName: "UnidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Metas_UnidadeId",
                table: "Metas",
                column: "UnidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Metas_Unidades_UnidadeId",
                table: "Metas",
                column: "UnidadeId",
                principalTable: "Unidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Metas_Unidades_UnidadeId",
                table: "Metas");

            migrationBuilder.DropIndex(
                name: "IX_Metas_UnidadeId",
                table: "Metas");

            migrationBuilder.RenameColumn(
                name: "UnidadeId",
                table: "Metas",
                newName: "UserId");
        }
    }
}
