using JKCASH.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Interfaces
{
    public interface ICategoriaRepositorio
    {
        void Adicionar(Categoria categoria);

        List<Categoria> ListarTodas();

        bool Existe(Guid id);
    }
}
