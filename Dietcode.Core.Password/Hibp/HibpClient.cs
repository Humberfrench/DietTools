using System.Net;

namespace Dietcode.Core.Password.Hibp;

/// <summary>
/// Typed client para a API range do Pwned Passwords. Só recebe/envia o
/// prefixo de 5 caracteres do hash — nunca a senha, o hash completo ou o
/// sufixo.
/// </summary>
public sealed class HibpClient
{
    private readonly HttpClient _httpClient;

    public HibpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Corpo bruto da resposta (linhas "SUFFIX:COUNT"), ou <c>null</c> se o
    /// provider não respondeu com sucesso após a tentativa de retry.
    /// </summary>
    public async Task<string?> GetRangeAsync(string prefix, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"range/{prefix}", cancellationToken);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync(cancellationToken);

                if (!IsTransient(response.StatusCode) || attempt == 2)
                    return null;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                if (attempt == 2)
                    return null;
            }

            // Retry único, só para falha transitória (5xx ou erro de rede) — nunca para 4xx.
            await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken);
        }

        return null;
    }

    private static bool IsTransient(HttpStatusCode statusCode) => (int)statusCode >= 500;
}
