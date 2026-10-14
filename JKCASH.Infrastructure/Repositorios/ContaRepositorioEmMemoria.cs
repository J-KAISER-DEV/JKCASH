using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Infrastructure.Repositorios
{
    public class ContaRepositorioEmMemoria : IContaRepositorio
    {
        private readonly Dictionary<Guid, Conta> _contas = new();

        public void Adicionar(Conta conta)
        {
            ArgumentNullException.ThrowIfNull(conta);
            foreach(Conta existente in _contas.Values)
            {
                bool mesmoNome = string.Equals(
                    existente.Nome,
                    conta.Nome,
                    StringComparison.OrdinalIgnoreCase
                    );
                bool mesmoBanco = string.Equals(
                    existente.Banco,
                    conta.Banco,
                    StringComparison.OrdinalIgnoreCase
                    );
                if (mesmoNome && mesmoBanco)
                {
                    throw new ArgumentException(
                        "Conta ja existente mesmo banco e nome");
                }
            }
            _contas.Add(conta.Id, conta);
        }

        public List<Conta> ListarTodas()
        {
            return _contas.Values.ToList();
        }
    }
}
