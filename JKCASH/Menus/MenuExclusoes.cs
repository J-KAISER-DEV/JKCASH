using JKCASH.Application.Modelos;
using JKCASH.Application.Servicos;

namespace JKCASH.Menus;

public sealed class MenuExclusoes(ExclusaoServico servico)
{
    public void Executar() => NavegacaoConsole.Executar("Excluir registros", "Voltar",
        ("Contas e suas transações", () => Excluir(TipoRegistro.Conta)),
        ("Categorias", () => Excluir(TipoRegistro.Categoria)),
        ("Transações (reverte o saldo)", () => Excluir(TipoRegistro.Transacao)),
        ("Transferências (as duas movimentações)", () => Excluir(TipoRegistro.Transferencia)),
        ("Cartões e todo o histórico vinculado", () => Excluir(TipoRegistro.Cartao)),
        ("Compras do cartão e suas parcelas", () => Excluir(TipoRegistro.Compra)),
        ("Faturas e as compras relacionadas", () => Excluir(TipoRegistro.Fatura)),
        ("Pagamentos de faturas (reverte o pagamento)", () => Excluir(TipoRegistro.Pagamento)),
        ("Parcelas (remove a compra completa)", () => Excluir(TipoRegistro.Parcela)));

    private void Excluir(TipoRegistro tipo)
    {
        List<ItemExclusao> itens = servico.Listar(tipo);
        if (itens.Count == 0)
        {
            Console.WriteLine("Não há registros deste tipo.");
            return;
        }
        for (int i = 0; i < itens.Count; i++) Console.WriteLine($"{i + 1} - {itens[i].Descricao}");
        string escolha = LeituraConsole.LerTexto("Número do registro (0 para cancelar): ");
        if (escolha == "0") return;
        if (!int.TryParse(escolha, out int numero) || numero < 1 || numero > itens.Count)
            throw new ArgumentException("Escolha um número da lista.");

        PreviaExclusao previa = servico.Preparar(tipo, itens[numero - 1].Id);
        Console.WriteLine($"\nRegistro: {previa.Descricao}\n{previa.Impacto}");
        if (LeituraConsole.LerTexto("Para excluir definitivamente, digite EXCLUIR: ") != "EXCLUIR")
        {
            Console.WriteLine("Exclusão cancelada.");
            return;
        }
        servico.Excluir(previa);
        Console.WriteLine("Registros excluídos e valores atualizados.");
    }
}
