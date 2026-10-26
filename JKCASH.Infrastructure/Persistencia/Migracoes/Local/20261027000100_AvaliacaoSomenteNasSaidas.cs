using Microsoft.EntityFrameworkCore.Migrations;

namespace JKCASH.Infrastructure.Persistencia.Migracoes.Local;

public partial class AvaliacaoSomenteNasSaidas : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>("Importancia", "Transacoes", schema: "dbo",
            type: "INTEGER", nullable: true, oldClrType: typeof(int), oldType: "INTEGER");
        migrationBuilder.AlterColumn<string>("Justificativa", "Transacoes", schema: "dbo",
            type: "TEXT", maxLength: 500, nullable: true, oldClrType: typeof(string),
            oldType: "TEXT", oldMaxLength: 500);
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("A reversão exige tratar explicitamente as entradas sem avaliação; os dados não serão preenchidos artificialmente.");
}
