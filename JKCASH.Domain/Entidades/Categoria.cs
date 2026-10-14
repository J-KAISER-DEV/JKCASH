using System;
using System.Collections.Generic;
using System.Text;

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
            if (id == Guid.Empty)
            {
                throw new ArgumentException
                    ("Informe o identificador da categoria", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException
                    ("Informe o nome da categoria", nameof (nome));
            }
            Id = id;
            Nome = nome.Trim();
        }
        public static Categoria Restaurar(Guid id, string nome)
        {
            return new Categoria(id, nome);
        }
    }
}
