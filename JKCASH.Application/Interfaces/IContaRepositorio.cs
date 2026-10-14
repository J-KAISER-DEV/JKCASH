using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface IContaRepositorio
    {
        void Adicionar(Conta conta);

        List<Conta> ListarTodas();

    }
}
