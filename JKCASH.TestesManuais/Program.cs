using System.Globalization;
using JKCASH.TestesManuais.Cenarios;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

if (args.Contains("--exclusao-contas"))
{
    TesteExclusaoConta.Executar(usarSql: false);
    TesteExclusaoConta.Executar(usarSql: true);
    return;
}

// Cada classe organiza os cenários de uma parte da aplicação.
TesteCartaoCredito.Executar();
TesteVinculoCartaoConta.Executar();
TesteRepositorioCartao.Executar();
TesteCompraCartao.Executar();
TesteFaturasParcelasPagamentos.Executar();
TesteCartaoCreditoSql.Executar();
