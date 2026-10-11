using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Infrastructure.Repositorios
{
    public class ContaRepositorioEmMemoria
    {
        private readonly Dictionary<Guid, Conta> _contas = new();

        public void Adicionar(Conta conta)
        {
            _contas.Add(conta.Id, conta);
        }
    }
}
