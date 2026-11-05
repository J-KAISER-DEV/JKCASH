using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Application.Modelos
{
    public sealed record GastoPorCategoria(
        Guid CategoriaId,
        string Categoria,
        int Quantidade,
        decimal Total);
  
}
