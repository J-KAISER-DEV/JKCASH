using System.Text.Json;
using JKCASH.Application.Modelos;

namespace JKCASH.Infrastructure.Backup;

public sealed class HistoricoBackup(string caminho)
{
    public List<RegistroBackup> Ler()
    {
        if (!File.Exists(caminho)) return [];
        return JsonSerializer.Deserialize<List<RegistroBackup>>(File.ReadAllText(caminho))
            ?? throw new InvalidDataException("O histórico local não pôde ser lido.");
    }

    public void Registrar(string operacao, string estado, Guid? id, string mensagem)
    {
        var itens = Ler();
        itens.Insert(0, new(DateTimeOffset.UtcNow, operacao, estado, id, mensagem));
        Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);
        string temporario = caminho + ".tmp";
        File.WriteAllText(temporario, JsonSerializer.Serialize(itens.Take(200)));
        File.Move(temporario, caminho, overwrite: true);
    }
}
