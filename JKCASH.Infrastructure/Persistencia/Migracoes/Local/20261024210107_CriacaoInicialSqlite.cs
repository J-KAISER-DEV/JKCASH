using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JKCASH.Infrastructure.Persistencia.Migracoes.Local
{
    /// <inheritdoc />
    public partial class CriacaoInicialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Categorias",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Banco = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Destinacao = table.Column<int>(type: "INTEGER", nullable: false),
                    SaldoInicial = table.Column<long>(type: "INTEGER", nullable: false),
                    SaldoAtual = table.Column<long>(type: "INTEGER", nullable: false),
                    LimiteChequeEspecial = table.Column<long>(type: "INTEGER", nullable: false),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartoesCredito",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Limite = table.Column<long>(type: "INTEGER", nullable: false),
                    ValorUtilizado = table.Column<long>(type: "INTEGER", nullable: false),
                    DiaFechamento = table.Column<int>(type: "INTEGER", nullable: false),
                    DiaVencimento = table.Column<int>(type: "INTEGER", nullable: false),
                    CompraNoFechamentoNaProximaFatura = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartoesCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartoesCredito_Contas_ContaId",
                        column: x => x.ContaId,
                        principalSchema: "dbo",
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransferenciaId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<long>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Importancia = table.Column<int>(type: "INTEGER", nullable: false),
                    Planejamento = table.Column<int>(type: "INTEGER", nullable: true),
                    Urgencia = table.Column<int>(type: "INTEGER", nullable: true),
                    Justificativa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalSchema: "dbo",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transacoes_Contas_ContaId",
                        column: x => x.ContaId,
                        principalSchema: "dbo",
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprasCartao",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CartaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Valor = table.Column<long>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Importancia = table.Column<int>(type: "INTEGER", nullable: false),
                    Planejamento = table.Column<int>(type: "INTEGER", nullable: false),
                    Urgencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Justificativa = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    QuantidadeParcelas = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasCartao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasCartao_CartoesCredito_CartaoId",
                        column: x => x.CartaoId,
                        principalSchema: "dbo",
                        principalTable: "CartoesCredito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasCartao_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalSchema: "dbo",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FaturasCartao",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CartaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Fechamento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Vencimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Encerramento = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaturasCartao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaturasCartao_CartoesCredito_CartaoId",
                        column: x => x.CartaoId,
                        principalSchema: "dbo",
                        principalTable: "CartoesCredito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosFatura",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FaturaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TransacaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Valor = table.Column<long>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosFatura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosFatura_Contas_ContaId",
                        column: x => x.ContaId,
                        principalSchema: "dbo",
                        principalTable: "Contas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagamentosFatura_FaturasCartao_FaturaId",
                        column: x => x.FaturaId,
                        principalSchema: "dbo",
                        principalTable: "FaturasCartao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagamentosFatura_Transacoes_TransacaoId",
                        column: x => x.TransacaoId,
                        principalSchema: "dbo",
                        principalTable: "Transacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParcelasCartao",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompraId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CartaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Numero = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Valor = table.Column<long>(type: "INTEGER", nullable: false),
                    FaturaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParcelasCartao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParcelasCartao_CartoesCredito_CartaoId",
                        column: x => x.CartaoId,
                        principalSchema: "dbo",
                        principalTable: "CartoesCredito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParcelasCartao_ComprasCartao_CompraId",
                        column: x => x.CompraId,
                        principalSchema: "dbo",
                        principalTable: "ComprasCartao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParcelasCartao_FaturasCartao_FaturaId",
                        column: x => x.FaturaId,
                        principalSchema: "dbo",
                        principalTable: "FaturasCartao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartoesCredito_ContaId",
                schema: "dbo",
                table: "CartoesCredito",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasCartao_CartaoId",
                schema: "dbo",
                table: "ComprasCartao",
                column: "CartaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasCartao_CategoriaId",
                schema: "dbo",
                table: "ComprasCartao",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_FaturasCartao_CartaoId",
                schema: "dbo",
                table: "FaturasCartao",
                column: "CartaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosFatura_ContaId",
                schema: "dbo",
                table: "PagamentosFatura",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosFatura_FaturaId",
                schema: "dbo",
                table: "PagamentosFatura",
                column: "FaturaId");

            migrationBuilder.CreateIndex(
                name: "UQ_PagamentosFatura_Transacao",
                schema: "dbo",
                table: "PagamentosFatura",
                column: "TransacaoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParcelasCartao_CartaoId",
                schema: "dbo",
                table: "ParcelasCartao",
                column: "CartaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ParcelasCartao_FaturaId",
                schema: "dbo",
                table: "ParcelasCartao",
                column: "FaturaId");

            migrationBuilder.CreateIndex(
                name: "UQ_ParcelasCartao_Compra_Numero",
                schema: "dbo",
                table: "ParcelasCartao",
                columns: new[] { "CompraId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CategoriaId",
                schema: "dbo",
                table: "Transacoes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaId",
                schema: "dbo",
                table: "Transacoes",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_TransferenciaId",
                schema: "dbo",
                table: "Transacoes",
                column: "TransferenciaId",
                filter: "[TransferenciaId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagamentosFatura",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ParcelasCartao",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Transacoes",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ComprasCartao",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "FaturasCartao",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Categorias",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CartoesCredito",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Contas",
                schema: "dbo");
        }
    }
}
