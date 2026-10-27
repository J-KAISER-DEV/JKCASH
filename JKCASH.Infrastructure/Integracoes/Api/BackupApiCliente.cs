using System.Net.Http.Json;
using JKCASH.Application.Modelos;

namespace JKCASH.Infrastructure.Integracoes.Api;

public sealed class BackupApiCliente(HttpClient http)
{
    public async Task<BackupPacote> ReceberDadosSqlAsync(CancellationToken ct) =>
        await http.GetFromJsonAsync<BackupPacote>("api/exportacao/sqlserver", ct)
        ?? throw new InvalidDataException("A API não retornou os dados existentes no SQL Server.");

    public async Task<List<BackupResumo>> ListarAsync(CancellationToken ct) =>
        await http.GetFromJsonAsync<List<BackupResumo>>("api/backups", ct)
        ?? throw new InvalidDataException("A API não retornou a lista de backups.");

    public async Task<BackupResumo> EnviarAsync(BackupEnvio envio, CancellationToken ct)
    {
        using var resposta = await http.PutAsJsonAsync($"api/backups/{envio.Id}", envio, ct);
        resposta.EnsureSuccessStatusCode();
        return await resposta.Content.ReadFromJsonAsync<BackupResumo>(ct)
            ?? throw new InvalidDataException("A API não confirmou o armazenamento.");
    }

    public async Task<BackupPacote> ReceberAsync(Guid id, CancellationToken ct) =>
        await http.GetFromJsonAsync<BackupPacote>($"api/backups/{id}", ct)
        ?? throw new InvalidDataException("A API não retornou o backup escolhido.");
}
