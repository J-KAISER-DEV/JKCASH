using JKCASH.Domain.Entidades;
using JKCASH.Domain.Enums;
using JKCASH.Infrastructure.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
Dictionary<Guid,Conta> contas = new Dictionary<Guid,Conta>();
//PROGRAN TESTE

Conta contaTeste = 
    new Conta 
    (nome: "Conta Teste",
     banco:"Banco teste",
     tipo : TipoConta.Corrente,
     saldoInicial: 100m
        );

Conta contaTeste2 =
    new Conta
    (nome: "Conta Teste2",
     banco: "Banco teste2",
     tipo: TipoConta.Corrente,
     saldoInicial: 1000m
        );
Conta contaTeste3 =
    new Conta
    (nome: "Conta Teste3",
     banco: "Banco teste3",
     tipo: TipoConta.Corrente,
     saldoInicial: 1200m
        );
Conta contaTeste4 =
    new Conta
    (nome: "Conta Teste4",
     banco: "Banco teste4",
     tipo: TipoConta.Corrente,
     saldoInicial: 1030m
        );

contas.Add(contaTeste.Id, contaTeste);
contas.Add(contaTeste2.Id, contaTeste2);
contas.Add(contaTeste3.Id, contaTeste3);
contas.Add(contaTeste4.Id, contaTeste4);

Console.WriteLine($"Contas Cadastradas:{contas.Count}");

foreach (Conta conta in contas.Values)
{
    Console.WriteLine(
        $"{conta.Nome} | {conta.Banco} | {conta.Saldo.SaldoAtual:C2}");
}
//Console.WriteLine($"Nome: {contaTeste.Nome}\n" +
//    $"Banco: {contaTeste.Banco}\n" +
//    $"Saldo: {contaTeste.Saldo.SaldoAtual:C2}\n");


//Teste do repositorio de transacao
TransacaoRepositoEmMemoria repositorio = new();
try
{
    Transacao transacao = 
}

