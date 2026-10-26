namespace JKCASH.Application.Excecoes;

// A operação não foi confirmada. O chamador deve consultar novamente antes de tentar outra vez.
public sealed class ConflitoFinanceiroException : InvalidOperationException
{
    public ConflitoFinanceiroException(string mensagem) : base(mensagem) { }

    public ConflitoFinanceiroException(string mensagem, Exception causa) : base(mensagem, causa) { }
}
