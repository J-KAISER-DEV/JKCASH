using JKCASH.Application.Interfaces;
using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Infrastructure.Repositorios
{
    public class CategoriaRepositorioEmMemoria : ICategoriaRepositorio
    {
        private Dictionary<Guid, Categoria> _categorias = new();
        internal void Substituir(Dictionary<Guid, Categoria> categorias) => _categorias = categorias;

        public bool Existe(Guid id)
        {
            return _categorias.ContainsKey(id);
        }

        public void Adicionar(Categoria categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);

            foreach(Categoria existente in _categorias.Values)
            {
                if(string.Equals(
                    existente.Nome,
                    categoria.Nome,
                    StringComparison.OrdinalIgnoreCase
                    ))
                {
                    throw new ArgumentException(
                        "Ja existe uma categoria com esse nome");
                }
            }
            _categorias.Add(categoria.Id, categoria);
        }
        public List<Categoria> ListarTodas()
        {
            return _categorias.Values.ToList();
        }
    }
}
