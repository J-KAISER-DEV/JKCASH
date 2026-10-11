using JKCASH.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JKCASH.Domain.Entidades
{
    public sealed class Transacao
    {
        public Guid Id { get; private set; }
        public Guid ContaId { get; private set; }
        public TipoTransacao Tipo {  get; private set; }
        public decimal Valor { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public DateTime Data {  get; private set; }
        public CategoriaTransacao Categoria { get; private set; }

        public Transacao(
        Guid contaId,
        TipoTransacao tipo,
        decimal valor,
        string descricao,
        DateTime data,
        CategoriaTransacao categoria)
        {
            if (contaId == Guid.Empty)
            {
                throw new ArgumentNullException(
                    "Informe o identificador da conta. ",nameof(contaId));
            }

            if(!Enum.IsDefined(typeof(TipoTransacao),tipo))
            {
                throw new ArgumentException(
                    "Tipo de transação invalido. ", nameof(tipo));
            }

            if(valor <= 0m || valor != decimal.Round(valor,2))
            {
                throw new ArgumentException(
                    "O valor deve ser positivo e ter no maximo duas casa decimais. ",
                    nameof(valor));
            }
            if(string.IsNullOrWhiteSpace(descricao))
            {
                throw new ArgumentException(
                    "Informe a descrição", nameof (descricao));
            }
            if(!Enum.IsDefined(typeof(CategoriaTransacao),categoria))
            {
                throw new ArgumentException(
                    "Categoria Invalida. ", nameof(categoria));
            }

            Id = Guid.NewGuid();
            ContaId = contaId;
            Tipo = tipo;
            Valor = valor;
            Descricao = descricao;
            Data = data;
            Categoria = categoria;
        }
    }
}
