using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Entidades;
using JKCASH.Application.Interfaces;
namespace JKCASH.Infrastructure.Repositorios
{
    public class TransacaoRepositoEmMemoria : ITransacaoRepositorio
    {
        private Dictionary<Guid, Transacao> _transacoes = new();

        internal void AdicionarTransferencia(Transferencia transferencia)
        {
            // Uma falha ao preparar qualquer registro preserva o dicionário original.
            var transacoes = new Dictionary<Guid, Transacao>(_transacoes);
            transacoes.Add(transferencia.Saida.Id, transferencia.Saida);
            transacoes.Add(transferencia.Entrada.Id, transferencia.Entrada);
            _transacoes = transacoes;
        }

        public List<Transacao> ListarPorPeriodo(
            List<Guid> contaIds,
            DateTime dataInicial,
            DateTime dataFinal)
        {
            ArgumentNullException.ThrowIfNull(contaIds);

            if (dataInicial.Date > dataFinal.Date)
            {
                throw new ArgumentException(
                    "A data inicial não pode ser posterior à data final.");
            }

            List<Transacao> resultado = new();

            foreach (Transacao transacao in _transacoes.Values)
            {
                bool contaSelecionada = contaIds.Contains(transacao.ContaId);

                bool dentroDoPeriodo =
                    transacao.Data.Date >= dataInicial.Date &&
                    transacao.Data.Date <= dataFinal.Date;

                if (contaSelecionada && dentroDoPeriodo)
                {
                    resultado.Add(transacao);
                }
            }

            return resultado
                .OrderBy(transacao => transacao.Data)
                .ThenBy(transacao => transacao.Id)
                .ToList();
        }
        internal void RemoverPorConta(Guid contaId)
        {
            if (_transacoes.Values.Any(t => t.ContaId == contaId && t.TransferenciaId != null))
                throw new InvalidOperationException("A conta participa de transferências. A exclusão apagaria parte do histórico de outra conta.");
            // Prepara a cópia antes de substituir os dados da execução.
            var restantes = _transacoes.Where(item => item.Value.ContaId != contaId)
                .ToDictionary(item => item.Key, item => item.Value);
            _transacoes = restantes;
        }

        public void Adicionar(Transacao transacao, Conta conta)
        {
            ArgumentNullException.ThrowIfNull(transacao);
            ArgumentNullException.ThrowIfNull(conta);

            if (transacao.ContaId != conta.Id)
                throw new ArgumentException("A transação pertence a outra conta.");

            // Na memória, a conta recebida pelo serviço já foi movimentada.
            Adicionar(transacao);
        }
        
        public void Adicionar(Transacao transacao)
        {
            ArgumentNullException.ThrowIfNull(transacao);
            _transacoes.Add(transacao.Id, transacao);
        }


        public Transacao? BuscarPorId(Guid id)
        {
            if(_transacoes.TryGetValue(id,out Transacao? transacao))
            {
                return transacao;
            }
            return null;
        }

        public List<Transacao> ListarTodas()
        {
            return _transacoes.Values.ToList();
        }

        public List<Transacao> ListarPorConta(Guid contaid)
        {
            List<Transacao> resultado = new();
            foreach (Transacao transacao in _transacoes.Values)
            {
                if (transacao.ContaId == contaid)
                {
                    resultado.Add(transacao);
                }
            }
            return resultado;
        }
    }
}
