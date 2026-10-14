using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Entidades;
using JKCASH.Application.Interfaces;
namespace JKCASH.Infrastructure.Repositorios
{
    public class TransacaoRepositoEmMemoria : ITransacaoRepositorio
    {
        private readonly Dictionary<Guid, Transacao> _transacoes = new();

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
