using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface ITransacaoRepositorio
    {
        void Adicionar(Transacao transacao);
    }
}
