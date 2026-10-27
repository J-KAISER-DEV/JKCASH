using System.Security.Cryptography;
using JKCASH.Application.Modelos;

namespace JKCASH.Infrastructure.Backup;

public sealed class BackupLocal
{
    public string CaminhoBanco { get; }
    public string PastaSeguranca => Path.Combine(Path.GetDirectoryName(CaminhoBanco)!, "BackupsSeguranca");

    public BackupLocal(string caminhoBanco) => CaminhoBanco = Path.GetFullPath(caminhoBanco);

    public BackupEnvio PrepararEnvio()
    {
        if (!File.Exists(CaminhoBanco))
            throw new InvalidOperationException("Ainda não existe banco local. Abra o aplicativo para começar.");
        string temporario = Path.Combine(Path.GetTempPath(), $"jkcash-envio-{Guid.NewGuid():N}.db");
        try
        {
            BancoBackup.Copiar(CaminhoBanco, temporario);
            BancoBackup.Conferir(temporario);
            byte[] banco = File.ReadAllBytes(temporario);
            return new BackupEnvio(Guid.NewGuid(), Environment.MachineName, BancoBackup.VersaoFormato,
                Convert.ToHexString(SHA256.HashData(banco)), banco);
        }
        finally { ExcluirTemporario(temporario); }
    }

    public static void ConferirPacote(BackupPacote pacote)
    {
        if (pacote.Resumo.VersaoFormato != BancoBackup.VersaoFormato || pacote.Resumo.Id == Guid.Empty)
            throw new InvalidDataException("Formato de backup não reconhecido.");
        if (pacote.Banco.Length != pacote.Resumo.TamanhoBytes || pacote.Banco.Length > BancoBackup.LimiteBytes ||
            !string.Equals(Convert.ToHexString(SHA256.HashData(pacote.Banco)), pacote.Resumo.Sha256,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("O conteúdo recebido difere do backup armazenado. Nada foi restaurado.");
    }

    public ConferenciaBackup ConferirRecebido(BackupPacote pacote)
    {
        ConferirPacote(pacote);
        string caminho = Path.Combine(Path.GetTempPath(), $"jkcash-conferencia-{Guid.NewGuid():N}.db");
        try { File.WriteAllBytes(caminho, pacote.Banco); return BancoBackup.Conferir(caminho); }
        finally { ExcluirTemporario(caminho); }
    }

    // Chamar somente na tela inicial, antes de abrir qualquer contexto do aplicativo.
    public string? Restaurar(BackupPacote pacote)
    {
        ConferirPacote(pacote);
        string pasta = Path.GetDirectoryName(CaminhoBanco)!;
        Directory.CreateDirectory(pasta);
        string preparado = Path.Combine(pasta, $"restauracao-{Guid.NewGuid():N}.db");
        try
        {
            File.WriteAllBytes(preparado, pacote.Banco);
            BancoBackup.AtualizarCopia(preparado);
            string? seguranca = null;
            if (File.Exists(CaminhoBanco))
            {
                Directory.CreateDirectory(PastaSeguranca);
                seguranca = Path.Combine(PastaSeguranca, $"antes-restauracao-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.db");
                BancoBackup.Copiar(CaminhoBanco, seguranca);
                BancoBackup.Conferir(seguranca);
                // Garante que o arquivo anterior não deixe um WAL para o banco recebido.
                BancoBackup.PrepararTroca(CaminhoBanco);
                File.Replace(preparado, CaminhoBanco, null);
            }
            else File.Move(preparado, CaminhoBanco);
            return seguranca;
        }
        finally { ExcluirTemporario(preparado); }
    }

    private static void ExcluirTemporario(string caminho)
    {
        // Apenas arquivos privados desta operação; nunca o banco do usuário.
        foreach (string sufixo in new[] { "", "-wal", "-shm" })
        {
            try { File.Delete(caminho + sufixo); }
            catch (IOException erro) { System.Diagnostics.Debug.WriteLine(erro); }
            catch (UnauthorizedAccessException erro) { System.Diagnostics.Debug.WriteLine(erro); }
        }
    }
}
