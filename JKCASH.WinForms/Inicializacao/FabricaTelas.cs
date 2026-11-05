using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Infrastructure.Persistencia;
using JKCASH.Infrastructure.Consultas;
using JKCASH.WinForms.Telas;
using JKCASH.Infrastructure.Integracoes.Api;
namespace JKCASH.WinForms.Inicializacao;

public static class FabricaTelas
{
    public static UcCategorias CriarCategorias()
    {
        ICategoriaRepositorio repositorio=
            new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);

        return new UcCategorias(repositorio);
    }

    private static readonly HttpClient HttpApi = new()
    {
        BaseAddress = new Uri("https://localhost:7068/"),
        Timeout = TimeSpan.FromSeconds(60)
    };

    private static ContasApiCliente CriarClienteContas()
    {
        return new ContasApiCliente(HttpApi);
    }

    public static FrmInicial CriarInicial()
    {
        return new FrmInicial(new ConexaoApiCliente(HttpApi), new BackupApiCliente(HttpApi),
            new JKCASH.Infrastructure.Backup.BackupLocal(ConexaoLocal.CaminhoBanco),
            new JKCASH.Infrastructure.Backup.HistoricoBackup(
                Path.Combine(ConexaoLocal.PastaDados, "historico-backups.json")));
    }


    public static UcContas CriarContas()
    {
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        return new UcContas(contas);
    }

    public static UcTransacoes CriarTransacoes()
    {
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        ICategoriaRepositorio categorias = new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);
        ITransacaoRepositorio transacoes = new TransacaoRepositorioEf(ConexaoLocal.CriarContexto);
        TransacaoServico servico = new(transacoes, categorias);

        ITransferenciaRepositorio repositorioTransferencias = new TransferenciaRepositorioEf(ConexaoLocal.CriarContexto);
        TransferenciaServico transferencias = new(contas, categorias, repositorioTransferencias);
        ICartaoCreditoRepositorio cartoes = new CartaoCreditoRepositorioEf(ConexaoLocal.CriarContexto);
        ICompraCartaoRepositorio compras = new CompraCartaoRepositorioEf(ConexaoLocal.CriarContexto);
        return new UcTransacoes(contas, categorias, servico, transferencias, cartoes,
            new CompraCartaoServico(cartoes, categorias, compras));
    }

    public static UcExtrato CriarExtrato()
    {
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        return new UcExtrato(
            contas,
            new ExtratoConsulta(ConexaoLocal.CriarContexto),
            new RelatorioGastosConsulta(ConexaoLocal.CriarContexto),
            new CategoriaRepositorioEf(ConexaoLocal.CriarContexto),
            new CartaoCreditoRepositorioEf(ConexaoLocal.CriarContexto));
    }

    public static UcCartoes CriarCartoes()
    {
        ICartaoCreditoRepositorio cartoes = new CartaoCreditoRepositorioEf(ConexaoLocal.CriarContexto);
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        return new UcCartoes(cartoes, contas);
    }

    public static UcFaturas CriarFaturas()
    {
        ICartaoCreditoRepositorio cartoes = new CartaoCreditoRepositorioEf(ConexaoLocal.CriarContexto);
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        ICategoriaRepositorio categorias = new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);
        // A mesma implementação oferece os contratos de compras e faturas.
        CompraCartaoRepositorioEf comprasEFaturas = new(ConexaoLocal.CriarContexto);
        PagamentoFaturaServico pagamentos = new(comprasEFaturas, cartoes, contas, categorias);

        return new UcFaturas(cartoes, comprasEFaturas, comprasEFaturas, contas, categorias, pagamentos);
    }

    public static UcDashboard CriarDashboard()
    {
        IDashboardConsulta consulta = new DashboardConsulta(ConexaoLocal.CriarContexto);
        return new UcDashboard(consulta);
    }
}
