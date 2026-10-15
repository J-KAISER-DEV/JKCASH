using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
namespace JKCASH.Domain.Entidades
{
    public sealed class Categoria
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }

        public Categoria(string nome)
             : this(Guid.NewGuid(), nome) { }

        private Categoria(Guid id, string nome)
        {
            Validacao.GuidNaoVazio(id, nameof(id));
            Validacao.TextoObrigatorio(nome, nameof(nome),100);
            Id = id;
            Nome = nome.Trim();
        }
        public static Categoria Restaurar(Guid id, string nome)
        {
            return new Categoria(id, nome);
        }
    }
}
