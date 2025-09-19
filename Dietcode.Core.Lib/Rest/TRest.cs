using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json.Serialization;
using Polly;
using Polly.Retry;

//HOW TO:
/*
 var rest = new TRest<ResponseModel, RequestModel>();

var response = await rest.Post(
    url: "https://api.exemplo.com/cliente",
    dataObject: new RequestModel { Nome = "Humberto" },
    headersFactory: () => new Dictionary<string, string>
    {
        { "X-App-Version", "1.0" },
        { "X-Request-ID", Guid.NewGuid().ToString() }
    },
    tokenBearer: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
);
 */

namespace Dietcode.Core.Lib.Rest
{

    public class TRest<T1, T2> where T1 : class, new() where T2 : class, new()
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> policy;

        private static JsonSerializerOptions defaultJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 4,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public TRest()
        {
            policy = Policy.Handle<HttpRequestException>()
                           .Or<TaskCanceledException>()
                           .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                           .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
        }

        public async Task<ApiResponse<T1>> Post(string url, T2 dataObject, Func<Dictionary<string, string>> headersFactory, string tokenBearer = "")
        {
            var jsonString = string.Empty;
            var result = new ApiResponse<T1>();
            try
            {
                using HttpClient client = CreateHttpClient(headersFactory, tokenBearer);

                var stringContent = new StringContent(dataObject.ToJson(), Encoding.UTF8, "application/json");

                var response = await policy.ExecuteAsync(() => client.PostAsync(url, stringContent));
                jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    result.Data = JsonSerializer.Deserialize<T1>(jsonString, defaultJsonOptions) ?? new T1();
                }
                result.RawContent = jsonString;
                result.Success = true;
                result.StatusCode = response.StatusCode;

                return result;
            }
            catch (Exception uex)
            {
                result.RawContent = jsonString;
                result.Success = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Erro = $"Mensagem de Erro => {uex.Message} ";
                return result!;
            }
        }

        public async Task<ApiResponse<T1>> Put(string url, T2 dataObject, Func<Dictionary<string, string>> headersFactory, string tokenBearer = "")
        {
            var jsonString = string.Empty;
            var result = new ApiResponse<T1>();
            try
            {
                using HttpClient client = CreateHttpClient(headersFactory, tokenBearer);

                var stringContent = new StringContent(dataObject.ToJson(), Encoding.UTF8, "application/json");

                var response = await policy.ExecuteAsync(() => client.PutAsync(url, stringContent));
                jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    result.Data = JsonSerializer.Deserialize<T1>(jsonString, defaultJsonOptions) ?? new T1();
                }
                result.RawContent = jsonString;
                result.Success = true;
                result.StatusCode = response.StatusCode;

                return result;
            }
            catch (Exception uex)
            {
                result.RawContent = jsonString;
                result.Success = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Erro = $"Mensagem de Erro => {uex.Message} ";
                return result!;
            }
        }

        public async Task<ApiResponse<T1>> Patch(string url, T2 dataObject, Func<Dictionary<string, string>> headersFactory, string tokenBearer = "")
        {
            var jsonString = string.Empty;
            var result = new ApiResponse<T1>();
            try
            {
                using HttpClient client = CreateHttpClient(headersFactory, tokenBearer);

                var stringContent = new StringContent(dataObject.ToJson(), Encoding.UTF8, "application/json");

                var response = await policy.ExecuteAsync(() => client.PatchAsync(url, stringContent));
                jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    result.Data = JsonSerializer.Deserialize<T1>(jsonString, defaultJsonOptions) ?? new T1();
                }
                result.RawContent = jsonString;
                result.Success = true;
                result.StatusCode = response.StatusCode;

                return result;
            }
            catch (Exception uex)
            {
                result.RawContent = jsonString;
                result.Success = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Erro = $"Mensagem de Erro => {uex.Message} ";
                return result!;
            }
        }

        public async Task<ApiResponse<T1>> Get(string url, Func<Dictionary<string, string>> headersFactory, string tokenBearer = "")
        {
            var jsonString = string.Empty;
            var result = new ApiResponse<T1>();
            try
            {
                using HttpClient client = CreateHttpClient(headersFactory, tokenBearer);

                var response = await policy.ExecuteAsync(() => client.GetAsync(url));
                jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    result.Data = JsonSerializer.Deserialize<T1>(jsonString, defaultJsonOptions) ?? new T1();
                }
                result.RawContent = jsonString;
                result.Success = true;
                result.StatusCode = response.StatusCode;

                return result;
            }
            catch (Exception uex)
            {
                result.RawContent = jsonString;
                result.Success = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Erro = $"Mensagem de Erro => {uex.Message} ";
                return result!;
            }
        }

        public async Task<ApiResponse<T1>> Get(string url, Dictionary<string, string> query, Func<Dictionary<string, string>> headersFactory, string tokenBearer = "")
        {
            var jsonString = string.Empty;
            var result = new ApiResponse<T1>();
            try
            {
                using HttpClient client = CreateHttpClient(headersFactory, tokenBearer);

                var queryString = string.Join("&", query.Select(q => $"{q.Key}={WebUtility.UrlEncode(q.Value)}"));
                url += "?" + queryString;

                var response = await policy.ExecuteAsync(() => client.GetAsync(url));
                jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    result.Data = JsonSerializer.Deserialize<T1>(jsonString, defaultJsonOptions) ?? new T1();
                }
                result.RawContent = jsonString;
                result.Success = true;
                result.StatusCode = response.StatusCode;

                return result;
            }
            catch (Exception uex)
            {
                result.RawContent = jsonString;
                result.Success = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Erro = $"Mensagem de Erro => {uex.Message} ";
                return result!;
            }
        }

        public async Task<ApiResponse<T1>> Delete(string url, Func<Dictionary<string, string>> headersFactory, string tokenBearer = "")
        {
            var result = new ApiResponse<T1>();
            var jsonString = string.Empty;
            try
            {
                using var client = CreateHttpClient(headersFactory, tokenBearer);

                var response = await policy.ExecuteAsync(() => client.DeleteAsync(url));
                jsonString = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    result.Data = JsonSerializer.Deserialize<T1>(jsonString, defaultJsonOptions) ?? new T1();
                }
                result.RawContent = jsonString;
                result.Success = true;
                result.StatusCode = response.StatusCode;

                return result;
            }
            catch (Exception uex)
            {
                result.RawContent = jsonString;
                result.Success = false;
                result.StatusCode = HttpStatusCode.BadRequest;
                result.Erro = $"Mensagem de Erro => {uex.Message} ";
                return result!;
            }
        }

        private HttpClient CreateHttpClient(Func<Dictionary<string, string>> headersFactory, string tokenBearer)
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            if (!tokenBearer.IsNullOrEmptyOrWhiteSpace())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer);
            }

            var headers = (headersFactory?.Invoke()) ?? new Dictionary<string, string>();
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            return client;
        }
    }
}

