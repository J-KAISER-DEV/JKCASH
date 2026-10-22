using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Infrastructure.Repositorios
{
    public class ContaRepositorioEmMemoria : IContaRepositorio
    {
        private Dictionary<Guid, Conta> _contas = new();
        internal void Substituir(Dictionary<Guid, Conta> contas) => _contas = contas;
        private readonly TransacaoRepositoEmMemoria? _transacoes;

        public ContaRepositorioEmMemoria(TransacaoRepositoEmMemoria? transacoes = null)
        {
            _transacoes = transacoes;
        }

        public void RemoverComTransacoes(Guid id)
        {
            if (!_contas.ContainsKey(id))
                throw new InvalidOperationException("Conta não encontrada.");
            if (_transacoes is null)
                throw new InvalidOperationException("O repositório de transações não foi configurado para a exclusão conjunta.");

            var restantes = new Dictionary<Guid, Conta>(_contas);
            restantes.Remove(id);
            _transacoes.RemoverPorConta(id);
            _contas = restantes;
        }

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

        public void Remover(Guid id)
        {
            if (!_contas.Remove(id))
                throw new InvalidOperationException("Conta não encontrada.");
        }
    }
}
