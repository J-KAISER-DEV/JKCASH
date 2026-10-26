using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Application.Interfaces;
using JKCASH.Application.Servicos;
using JKCASH.Infrastructure.Repositorios;
using JKCASH.Infrastructure.Persistencia;
using JKCASH.Infrastructure.Consultas;
using JKCASH.WinForms.Telas;
namespace JKCASH.WinForms.Inicializacao;

public static class FabricaTelas
{
    public static UcCategorias CriarCategorias()
    {
        ICategoriaRepositorio repositorio=
            new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);

        return new UcCategorias(repositorio);
    }

    public static UcContas CriarContas()
    {
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        ICategoriaRepositorio categorias = new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);
        ITransferenciaRepositorio repositorioTransferencias = new TransferenciaRepositorioEf(ConexaoLocal.CriarContexto);
        TransferenciaServico transferencias = new(contas, categorias, repositorioTransferencias);

        return new UcContas(contas, categorias, transferencias);
    }

    public static UcTransacoes CriarTransacoes()
    {
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        ICategoriaRepositorio categorias = new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);
        ITransacaoRepositorio transacoes = new TransacaoRepositorioEf(ConexaoLocal.CriarContexto);
        TransacaoServico servico = new(transacoes, categorias);

        return new UcTransacoes(contas, categorias, servico, transacoes);
    }

    public static UcCartoes CriarCartoes()
    {
        ICartaoCreditoRepositorio cartoes = new CartaoCreditoRepositorioEf(ConexaoLocal.CriarContexto);
        IContaRepositorio contas = new ContaRepositorioEf(ConexaoLocal.CriarContexto);
        ICategoriaRepositorio categorias = new CategoriaRepositorioEf(ConexaoLocal.CriarContexto);
        ICompraCartaoRepositorio compras = new CompraCartaoRepositorioEf(ConexaoLocal.CriarContexto);
        CompraCartaoServico servico = new(cartoes, categorias, compras);

        return new UcCartoes(cartoes, contas, categorias, compras, servico);
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
