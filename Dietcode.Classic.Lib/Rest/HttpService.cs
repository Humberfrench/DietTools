using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Dietcode.Classic.Lib.JsonConverting;

namespace Dietcode.Classic.Lib.Rest
{
    public static class HttpService
    {
        private static readonly JsonSerializerOptions jsonOptions = JsonOptionsFactory.CreateDefault();

        public static async Task<ApiResult<TResponse>> Post<TRequest, TResponse>(string url, TRequest payload,
                                                                                 EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                 string token = "", JsonSerializerOptions? options = null,
                                                                                 int secondsTimeout = 120,
                                                                                 CancellationToken cancellationToken = default)
                                                                                 where TResponse : class, new()
        {
            options ??= jsonOptions;

            string requestJson = JsonSerializer.Serialize(payload, options);

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Post<TResponse>(string url, EnumApiRest enumApiRest = EnumApiRest.None,
                                                                       string token = "", JsonSerializerOptions? options = null,
                                                                       int secondsTimeout = 120,
                                                                       CancellationToken cancellationToken = default)
                                                                       where TResponse : class, new()
        {
            options ??= jsonOptions;

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Put<TRequest, TResponse>(string url, TRequest payload,
                                                                                EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                string token = "", JsonSerializerOptions? options = null,
                                                                                int secondsTimeout = 120,
                                                                                CancellationToken cancellationToken = default)
                                                                                where TResponse : class, new()
        {
            options ??= jsonOptions;

            string requestJson = JsonSerializer.Serialize(payload, options);

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Put, url)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Get<TResponse>(string url, Dictionary<string, object> querystringParameter,
                                                                      EnumApiRest enumApiRest = EnumApiRest.None,
                                                                      string token = "", JsonSerializerOptions? options = null,
                                                                      int secondsTimeout = 120,
                                                                      CancellationToken cancellationToken = default)
                                                                      where TResponse : class, new()
        {
            options ??= jsonOptions;

            // 🔧 Construir querystring na URL
            if (querystringParameter != null && querystringParameter.Count > 0)
            {
                var querystring = string.Join("&",
                    querystringParameter
                        .Where(value => value.Value != null)
                        .Select(value => $"{Uri.EscapeDataString(value.Key)}={Uri.EscapeDataString(value.Value!.ToString()!)}"));

                url = url.Contains("?") ? $"{url}&{querystring}" : $"{url}?{querystring}";
            }
            return await Get<TResponse>(url, enumApiRest, token, options, secondsTimeout, cancellationToken);

        }



        public static async Task<ApiResult<TResponse>> Get<TResponse>(string url, EnumApiRest enumApiRest = EnumApiRest.None,
                                                                      string token = "", JsonSerializerOptions? options = null,
                                                                      int secondsTimeout = 120,
                                                                      CancellationToken cancellationToken = default)
                                                                      where TResponse : class, new()
        {
            options ??= jsonOptions;

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Patch<TRequest, TResponse>(string url, TRequest payload,
                                                                                  EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                  string token = "", JsonSerializerOptions? options = null,
                                                                                  string mediaType = "application/json",
                                                                                  int secondsTimeout = 120,
                                                                                  CancellationToken cancellationToken = default)
                                                                                  where TResponse : class, new()
        {
            options ??= jsonOptions;

            string requestJson = JsonSerializer.Serialize(payload, options);

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, mediaType)
            };

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Delete<TRequest, TResponse>(string url, TRequest payload,
                                                                                   EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                   string token = "", JsonSerializerOptions? options = null,
                                                                                   int secondsTimeout = 120,
                                                                                   CancellationToken cancellationToken = default)
                                                                                   where TResponse : class, new()
        {
            options ??= jsonOptions;

            string requestJson = JsonSerializer.Serialize(payload, options);

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Delete<TResponse>(string url, EnumApiRest enumApiRest = EnumApiRest.None,
                                                                         string token = "", JsonSerializerOptions? options = null,
                                                                         int secondsTimeout = 120,
                                                                         CancellationToken cancellationToken = default)
                                                                         where TResponse : class, new()
        {
            options ??= jsonOptions;

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Delete, url);

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        private static void ApplyHeaders(HttpRequestMessage request, EnumApiRest auth, string token)
        {
            switch (auth)
            {
                case EnumApiRest.Basic:
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
                    break;

                case EnumApiRest.Bearer:
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    break;

                case EnumApiRest.XApiKey:
                    request.Headers.Add("x-api-key", token);
                    break;

                case EnumApiRest.None:
                default:
                    break;
            }
        }


        private static async Task<ApiResult<TResponse>> ReadResponse<TResponse>(
            HttpResponseMessage response,
            JsonSerializerOptions options,
            CancellationToken cancellationToken = default)
            where TResponse : class, new()
        {
            var result = new ApiResult<TResponse>
            {
                StatusCode = response.StatusCode,
                TimeStamp = DateTime.UtcNow,
                IsSuccess = response.IsSuccessStatusCode,
                ContentType = response.Content.Headers.ContentType?.MediaType,
                ContentLength = response.Content.Headers.ContentLength
            };

            result.Content = response.Content is null
                ? string.Empty
                : await response.Content.ReadAsStringAsync();

            // Mensagem base em caso de erro (sem assumir JSON nenhum)
            if (!result.IsSuccess)
                result.Error = $"HTTP {(int)result.StatusCode} ({response.ReasonPhrase})";

            // Se tiver body e parecer JSON, tenta desserializar para TResponse
            if (!string.IsNullOrWhiteSpace(result.Content))
            {
                var bodyTrim = result.Content.TrimStart();
                var looksLikeJson =
                    (result.ContentType?.Contains("json", StringComparison.OrdinalIgnoreCase) ?? false) ||
                    bodyTrim.StartsWith("{") || bodyTrim.StartsWith("[");

                if (looksLikeJson)
                {
                    try
                    {
                        result.Data = JsonSerializer.Deserialize<TResponse>(result.Content, options) ?? new TResponse();
                    }
                    catch (Exception ex)
                    {
                        result.Error += $" - Falha ao desserializar JSON: {ex.Message}";
                        // Mantém Content para debug; não quebra fluxo
                    }
                }
            }

            return result;
        }
    }
}


