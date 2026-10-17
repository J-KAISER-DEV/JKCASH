using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface IContaRepositorio
    {
        void Adicionar(Conta conta);

        void Remover(Guid id);

        // A conta e suas transações são removidas juntas ou nada é removido.
        void RemoverComTransacoes(Guid id);

        List<Conta> ListarTodas();

    }
}
