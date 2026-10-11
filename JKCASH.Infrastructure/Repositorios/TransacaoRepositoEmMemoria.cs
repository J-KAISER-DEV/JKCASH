using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Entidades;
namespace JKCASH.Infrastructure.Repositorios
{
    public class TransacaoRepositoEmMemoria
    {
        private readonly Dictionary<Guid, Transacao> _transacoes = new();
        
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
    }
}
