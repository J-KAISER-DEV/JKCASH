using System;
using System.Collections.Generic;
using System.Text;
using JKCASH.Domain.Validacoes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace JKCASH.Infrastructure.Persistencia.Conversores;

public sealed class ConversorMoeda : ValueConverter<decimal, long>
{
    public ConversorMoeda()
        : base(
            valor => ParaCentavos(valor),
            centavos => centavos / 100m)
    { }

    private static long ParaCentavos(decimal valor)
    {
        Validacao.ValorMonetario(
            valor, nameof(valor), permitirNegativo: true);

        return checked((long)(valor * 100m));
    }
  
    
}
