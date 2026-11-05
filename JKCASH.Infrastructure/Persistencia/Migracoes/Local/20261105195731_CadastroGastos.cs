using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JKCASH.Infrastructure.Persistencia.Migracoes.Local
{
    /// <inheritdoc />
    public partial class CadastroGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GastoCadastroId",
                schema: "dbo",
                table: "Transacoes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GastoFrequencia",
                schema: "dbo",
                table: "Transacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GastoNatureza",
                schema: "dbo",
                table: "Transacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GastoNecessidade",
                schema: "dbo",
                table: "Transacoes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GastoNome",
                schema: "dbo",
                table: "Transacoes",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GastoCadastroId",
                schema: "dbo",
                table: "ComprasCartao",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GastoFrequencia",
                schema: "dbo",
                table: "ComprasCartao",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GastoNatureza",
                schema: "dbo",
                table: "ComprasCartao",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GastoNecessidade",
                schema: "dbo",
                table: "ComprasCartao",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GastoNome",
                schema: "dbo",
                table: "ComprasCartao",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Gastos",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CategoriaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Natureza = table.Column<int>(type: "INTEGER", nullable: false),
                    Importancia = table.Column<int>(type: "INTEGER", nullable: false),
                    Necessidade = table.Column<int>(type: "INTEGER", nullable: false),
                    Planejamento = table.Column<int>(type: "INTEGER", nullable: false),
                    Frequencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gastos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalSchema: "dbo",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_GastoCadastroId",
                schema: "dbo",
                table: "Transacoes",
                column: "GastoCadastroId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasCartao_GastoCadastroId",
                schema: "dbo",
                table: "ComprasCartao",
                column: "GastoCadastroId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_CategoriaId",
                schema: "dbo",
                table: "Gastos",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComprasCartao_Gastos_GastoCadastroId",
                schema: "dbo",
                table: "ComprasCartao",
                column: "GastoCadastroId",
                principalSchema: "dbo",
                principalTable: "Gastos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Gastos_GastoCadastroId",
                schema: "dbo",
                table: "Transacoes",
                column: "GastoCadastroId",
                principalSchema: "dbo",
                principalTable: "Gastos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComprasCartao_Gastos_GastoCadastroId",
                schema: "dbo",
                table: "ComprasCartao");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Gastos_GastoCadastroId",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropTable(
                name: "Gastos",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_GastoCadastroId",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_ComprasCartao_GastoCadastroId",
                schema: "dbo",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "GastoCadastroId",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "GastoFrequencia",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "GastoNatureza",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "GastoNecessidade",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "GastoNome",
                schema: "dbo",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "GastoCadastroId",
                schema: "dbo",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "GastoFrequencia",
                schema: "dbo",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "GastoNatureza",
                schema: "dbo",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "GastoNecessidade",
                schema: "dbo",
                table: "ComprasCartao");

            migrationBuilder.DropColumn(
                name: "GastoNome",
                schema: "dbo",
                table: "ComprasCartao");
        }
    }
}
