using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using JKCASH.Application.Modelos;
using JKCASH.Api.Modelos;

namespace JKCASH.Infrastructure.Integracoes.Api
{
    public sealed class ContasApiCliente
    {
        private readonly HttpClient _httpClient;

        public ContasApiCliente (HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient);
            _httpClient = httpClient;
        }
        public async Task<List<ContaResposta>> ListarAsync(
            CancellationToken cancellationToken = default)
        {
            var contas = await _httpClient
                .GetFromJsonAsync<List<ContaResposta>>(
                "api/contas",
                cancellationToken);

            return contas
                ?? throw new InvalidOperationException(
                    "A API retornou uma resposta sem os dados da conta. ");
        }
    }
}
