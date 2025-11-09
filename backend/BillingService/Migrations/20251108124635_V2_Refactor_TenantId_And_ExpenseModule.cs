using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Migrations
{
    /// <inheritdoc />
    public partial class V2_Refactor_TenantId_And_ExpenseModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faturas_Empresas_EmpresaId",
                table: "Faturas");

            migrationBuilder.DropForeignKey(
                name: "FK_Mensalistas_Empresas_EmpresaId",
                table: "Mensalistas");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitacoesAjuste_Faturamentos_FaturamentoId",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Faturamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioOperacoes",
                table: "UsuarioOperacoes");

            migrationBuilder.DropIndex(
                name: "IX_Mensalistas_EmpresaId",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Mensalistas");

            migrationBuilder.RenameColumn(
                name: "FaturamentoId",
                table: "SolicitacoesAjuste",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_SolicitacoesAjuste_FaturamentoId",
                table: "SolicitacoesAjuste",
                newName: "IX_SolicitacoesAjuste_TenantId");

            migrationBuilder.RenameColumn(
                name: "EmpresaId",
                table: "Faturas",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Faturas_EmpresaId",
                table: "Faturas",
                newName: "IX_Faturas_TenantId");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "UsuarioOperacoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UsuarioOperacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UsuarioOperacoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UsuarioOperacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RoleInOperation",
                table: "UsuarioOperacoes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UsuarioOperacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SolicitacoesAjuste",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "FaturamentoParcialId",
                table: "SolicitacoesAjuste",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SolicitacoesAjuste",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SolicitacoesAjuste",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Operacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Operacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Operacoes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Operacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Metas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Metas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Metas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Metas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Mensalistas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Mensalistas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Mensalistas",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Mensalistas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Faturas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Faturas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Faturas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioOperacoes",
                table: "UsuarioOperacoes",
                columns: new[] { "TenantId", "UserId", "OperacaoId" });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FaturamentosDiarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    ValorTotalConsolidado = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturamentosDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaturamentosDiarios_Operacoes_OperacaoId",
                        column: x => x.OperacaoId,
                        principalTable: "Operacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetodosPagamento",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetodosPagamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Operacoes_OperacaoId",
                        column: x => x.OperacaoId,
                        principalTable: "Operacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaturamentosParciais",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    HoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OperacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetodoPagamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturamentosParciais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaturamentosParciais_MetodosPagamento_MetodoPagamentoId",
                        column: x => x.MetodoPagamentoId,
                        principalTable: "MetodosPagamento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaturamentosParciais_Operacoes_OperacaoId",
                        column: x => x.OperacaoId,
                        principalTable: "Operacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesAjuste_FaturamentoParcialId",
                table: "SolicitacoesAjuste",
                column: "FaturamentoParcialId");

            migrationBuilder.CreateIndex(
                name: "IX_Operacoes_TenantId",
                table: "Operacoes",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Metas_TenantId",
                table: "Metas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensalistas_TenantId",
                table: "Mensalistas",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_TenantId",
                table: "ExpenseCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_OperacaoId",
                table: "Expenses",
                column: "OperacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentosDiarios_OperacaoId",
                table: "FaturamentosDiarios",
                column: "OperacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentosDiarios_TenantId",
                table: "FaturamentosDiarios",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentosParciais_MetodoPagamentoId",
                table: "FaturamentosParciais",
                column: "MetodoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentosParciais_OperacaoId",
                table: "FaturamentosParciais",
                column: "OperacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturamentosParciais_TenantId",
                table: "FaturamentosParciais",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MetodosPagamento_TenantId",
                table: "MetodosPagamento",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitacoesAjuste_FaturamentosParciais_FaturamentoParcialId",
                table: "SolicitacoesAjuste",
                column: "FaturamentoParcialId",
                principalTable: "FaturamentosParciais",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitacoesAjuste_FaturamentosParciais_FaturamentoParcialId",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "FaturamentosDiarios");

            migrationBuilder.DropTable(
                name: "FaturamentosParciais");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "MetodosPagamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioOperacoes",
                table: "UsuarioOperacoes");

            migrationBuilder.DropIndex(
                name: "IX_SolicitacoesAjuste_FaturamentoParcialId",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropIndex(
                name: "IX_Operacoes_TenantId",
                table: "Operacoes");

            migrationBuilder.DropIndex(
                name: "IX_Metas_TenantId",
                table: "Metas");

            migrationBuilder.DropIndex(
                name: "IX_Mensalistas_TenantId",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UsuarioOperacoes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UsuarioOperacoes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsuarioOperacoes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UsuarioOperacoes");

            migrationBuilder.DropColumn(
                name: "RoleInOperation",
                table: "UsuarioOperacoes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UsuarioOperacoes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropColumn(
                name: "FaturamentoParcialId",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SolicitacoesAjuste");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Operacoes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Metas");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Mensalistas");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Faturas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Faturas");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Faturas");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "SolicitacoesAjuste",
                newName: "FaturamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_SolicitacoesAjuste_TenantId",
                table: "SolicitacoesAjuste",
                newName: "IX_SolicitacoesAjuste_FaturamentoId");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "Faturas",
                newName: "EmpresaId");

            migrationBuilder.RenameIndex(
                name: "IX_Faturas_TenantId",
                table: "Faturas",
                newName: "IX_Faturas_EmpresaId");

            migrationBuilder.AddColumn<Guid>(
                name: "EmpresaId",
                table: "Mensalistas",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioOperacoes",
                table: "UsuarioOperacoes",
                columns: new[] { "UserId", "OperacaoId" });

            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiaVencimentoBoleto = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Faturamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    Origem = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturamentos_Operacoes_OperacaoId",
                        column: x => x.OperacaoId,
                        principalTable: "Operacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mensalistas_EmpresaId",
                table: "Mensalistas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturamentos_OperacaoId",
                table: "Faturamentos",
                column: "OperacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Faturas_Empresas_EmpresaId",
                table: "Faturas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mensalistas_Empresas_EmpresaId",
                table: "Mensalistas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitacoesAjuste_Faturamentos_FaturamentoId",
                table: "SolicitacoesAjuste",
                column: "FaturamentoId",
                principalTable: "Faturamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
