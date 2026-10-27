using System.Net.Http.Json;
using JKCASH.Application.Modelos;

namespace JKCASH.Infrastructure.Integracoes.Api;

public sealed class ConexaoApiCliente
{
    private readonly HttpClient _httpClient;

    public ConexaoApiCliente(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

    public async Task<bool> VerificarAsync(CancellationToken cancellationToken)
    {
        var resposta = await _httpClient.GetFromJsonAsync<EstadoApiResposta>(
            "api/status", cancellationToken);

        return resposta is { Servico: "JKCASH", Estado: "disponivel" };
    }
}
