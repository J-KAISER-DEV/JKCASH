using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Domain.Enums
{
    public enum TipoConta
    {
        Corrente = 1,
        Poupança = 2,
        Salario = 3,
        Investimento = 4,
        Outros = 5

    }

    public enum DestinacaoConta
    {
        UsoDiario = 1,
        Reserva = 2,
        Outros = 3
    }

    public enum TipoTransacao
    {
        Entrada = 1,
        Saida = 2
    }

    public enum ImportanciaTransacao
    {
        Baixa = 1,
        Media = 2,
        Alta = 3,
        Essencial = 4,
    }
    public enum CategoriaTransacao
    {
        Outros = 1,
        Saude = 2,
        Lazer = 3,
        Estudos = 4,
        Alimentacao = 5,
        Transporte = 6,
        Moradia = 7,
        Salario = 8
    }
}
