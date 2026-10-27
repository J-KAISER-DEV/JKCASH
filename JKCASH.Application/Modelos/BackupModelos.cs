namespace JKCASH.Application.Modelos;

public sealed record BackupResumo(Guid Id, DateTimeOffset CriadoEmUtc, string Maquina,
    int VersaoFormato, long TamanhoBytes, string Sha256);

public sealed record BackupEnvio(Guid Id, string Maquina, int VersaoFormato,
    string Sha256, byte[] Banco);

public sealed record BackupPacote(BackupResumo Resumo, byte[] Banco);

public sealed record ConferenciaBackup(IReadOnlyDictionary<string, long> Quantidades)
{
    public string Descricao => string.Join(Environment.NewLine,
        Quantidades.Select(item => $"{item.Key}: {item.Value}"));
}

public sealed record RegistroBackup(DateTimeOffset DataUtc, string Operacao, string Estado,
    Guid? BackupId, string Mensagem);
