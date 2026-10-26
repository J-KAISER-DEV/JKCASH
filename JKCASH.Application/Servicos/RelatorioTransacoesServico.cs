using JKCASH.Application.Interfaces;
using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Servicos
{
    public class RelatorioTransacoesServico
    {
        private readonly ITransacaoRepositorio _transacoes;

        public RelatorioTransacoesServico(
            ITransacaoRepositorio transacoes)
        {
            ArgumentNullException.ThrowIfNull(transacoes);

            _transacoes = transacoes;
        }
        public Dictionary<Guid, decimal> ResumirSaidasPorCategoria(
            List<Guid> contaIds,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            var transacoes = _transacoes.ListarPorPeriodo(
                contaIds, dataInicial, dataFinal);

            return transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Saida && transacao.TransferenciaId == null)
                .GroupBy(transacao => transacao.CategoriaId)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(transacao => transacao.Valor));
        }

        public Dictionary<string, decimal> ResumirSaidasPorPlanejamento(
            List<Guid> contaIds,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            var transacoes = _transacoes.ListarPorPeriodo(
                contaIds, dataInicial, dataFinal);

            return transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Saida && transacao.TransferenciaId == null)
                .GroupBy(transacao =>
                    transacao.Planejamento?.ToString() ?? "Não informado")
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(transacao => transacao.Valor));
        }

        public Dictionary<string, decimal> ResumirSaidasPorImportancia(
            List<Guid> contaIds,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            var transacoes = _transacoes.ListarPorPeriodo(
                contaIds, dataInicial, dataFinal);

            return transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Saida && transacao.TransferenciaId == null)
                .GroupBy(transacao => transacao.Importancia?.ToString() ?? "Não informada")
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(transacao => transacao.Valor));
        }

        public Dictionary<string, decimal> ResumirSaidasPorUrgencia(
            List<Guid> contaIds,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            var transacoes = _transacoes.ListarPorPeriodo(
                contaIds, dataInicial, dataFinal);

            return transacoes
                .Where(transacao => transacao.Tipo == TipoTransacao.Saida && transacao.TransferenciaId == null)
                .GroupBy(transacao => transacao.Urgencia?.ToString() ?? "Não informada")
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(transacao => transacao.Valor));
        }
    }
}
