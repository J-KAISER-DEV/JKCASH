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
        {
            if(string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException(
                    "Informe o nome da categoria", nameof(nome));
            }
            Id = Guid.NewGuid();
            Nome = nome.Trim();
        }
    }
}
