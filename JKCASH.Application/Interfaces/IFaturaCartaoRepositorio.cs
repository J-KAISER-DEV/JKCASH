using JKCASH.Domain.Entidades;

namespace JKCASH.Application.Interfaces;

public interface IFaturaCartaoRepositorio
{
    FaturaCartao? BuscarPorId(Guid id);
    List<FaturaCartao> ListarPorCartao(Guid cartaoId);

    // Contrato atômico: histórico da fatura e saída da conta são salvos juntos ou nada é salvo.
    void RegistrarPagamento(FaturaCartao original, FaturaCartao atualizada,
        Transacao transacao, Conta conta, CartaoCredito cartao);
}
