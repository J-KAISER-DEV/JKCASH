using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface ITransacaoRepositorio
    {
        // A transação e o saldo movimentado devem ser salvos juntos.
        void Adicionar(Transacao transacao, Conta conta);
        Transacao? BuscarPorId(Guid id);
        List<Transacao> ListarTodas();
        List<Transacao> ListarPorConta(Guid contaId);
    }
}
