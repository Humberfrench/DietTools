using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace Dietcode.Classic.Lib.Rest
{
    public static class HttpService
    {
        private static readonly JsonSerializerSettings jsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None
        };

        public static async Task<ApiResult<TResponse>> Post<TRequest, TResponse>(string url, TRequest payload,
                                                                                 EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                 string token = "", JsonSerializerSettings? options = null,
                                                                                 int secondsTimeout = 120,
                                                                                 CancellationToken cancellationToken = default)
                                                                                 where TResponse : class, new()
        {
            options ??= jsonSettings;

            string requestJson = JsonConvert.SerializeObject(payload, options);

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
                                                                       string token = "", JsonSerializerSettings? options = null,
                                                                       int secondsTimeout = 120,
                                                                       CancellationToken cancellationToken = default)
                                                                       where TResponse : class, new()
        {
            options ??= jsonSettings;

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Put<TRequest, TResponse>(string url, TRequest payload,
                                                                                EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                string token = "", JsonSerializerSettings? options = null,
                                                                                int secondsTimeout = 120,
                                                                                CancellationToken cancellationToken = default)
                                                                                where TResponse : class, new()
        {
            options ??= jsonSettings;

            string requestJson = JsonConvert.SerializeObject(payload, options);

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
                                                                      string token = "", JsonSerializerSettings? options = null,
                                                                      int secondsTimeout = 120,
                                                                      CancellationToken cancellationToken = default)
                                                                      where TResponse : class, new()
        {
            options ??= jsonSettings;

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
                                                                      string token = "", JsonSerializerSettings? options = null,
                                                                      int secondsTimeout = 120,
                                                                      CancellationToken cancellationToken = default)
                                                                      where TResponse : class, new()
        {
            options ??= jsonSettings;

            HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(secondsTimeout);
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            ApplyHeaders(request, enumApiRest, token);

            var response = await client.SendAsync(request, cancellationToken);

            return await ReadResponse<TResponse>(response, options, cancellationToken);
        }

        public static async Task<ApiResult<TResponse>> Patch<TRequest, TResponse>(string url, TRequest payload,
                                                                                  EnumApiRest enumApiRest = EnumApiRest.None,
                                                                                  string token = "", JsonSerializerSettings? options = null,
                                                                                  string mediaType = "application/json",
                                                                                  int secondsTimeout = 120,
                                                                                  CancellationToken cancellationToken = default)
                                                                                  where TResponse : class, new()
        {
            options ??= jsonSettings;

            string requestJson = JsonConvert.SerializeObject(payload, options);

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
                                                                                   string token = "", JsonSerializerSettings? options = null,
                                                                                   int secondsTimeout = 120,
                                                                                   CancellationToken cancellationToken = default)
                                                                                   where TResponse : class, new()
        {
            options ??= jsonSettings;

            string requestJson = JsonConvert.SerializeObject(payload, options);

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
                                                                         string token = "", JsonSerializerSettings? options = null,
                                                                         int secondsTimeout = 120,
                                                                         CancellationToken cancellationToken = default)
                                                                         where TResponse : class, new()
        {
            options ??= jsonSettings;

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
            JsonSerializerSettings options,
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

            if (!result.IsSuccess)
                result.Error = $"HTTP {(int)result.StatusCode} ({response.ReasonPhrase})";

            if (!string.IsNullOrWhiteSpace(result.Content))
            {
                var bodyTrim = result.Content.TrimStart();
                var looksLikeJson =
                    (result.ContentType?.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    bodyTrim.StartsWith("{") || bodyTrim.StartsWith("[");

                if (looksLikeJson)
                {
                    try
                    {
                        result.Data = JsonConvert.DeserializeObject<TResponse>(result.Content, options) ?? new TResponse();
                    }
                    catch (Exception ex)
                    {
                        result.Error += $" - Falha ao desserializar JSON: {ex.Message}";
                    }
                }
            }

            return result;
        }
    }
}
