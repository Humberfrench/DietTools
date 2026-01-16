using Dietcode.Api.Core.MiddleObjects;
using Dietcode.Core.Lib.Masking;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Dietcode.Api.Core.Middleware
{
    public sealed class ApiLoggingMiddleware(
        RequestDelegate next,
        IOptions<ApiLoggingOptions> options)
    {
        private readonly RequestDelegate _next = next;
        private readonly ApiLoggingOptions _options = options.Value;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            string? requestBody = null;

            if (context.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var originalBody = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Position = 0;
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalBody);

            var logEntry = new ApiLogEntry
            {
                Timestamp = DateTimeOffset.UtcNow,
                Method = context.Request.Method,
                Url = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                TraceId = context.TraceIdentifier,
                Request = SensitiveDataMasker.Mask(
                    string.IsNullOrWhiteSpace(requestBody)
                        ? null
                        : JsonSerializer.Deserialize<object>(requestBody)),
                Response = SensitiveDataMasker.Mask(
                    string.IsNullOrWhiteSpace(responseText)
                        ? null
                        : JsonSerializer.Deserialize<object>(responseText))
            };

            await WriteLogAsync(logEntry);
        }

        private async Task WriteLogAsync(ApiLogEntry entry)
        {
            Directory.CreateDirectory(_options.Directory);

            var file = Path.Combine(
                _options.Directory,
                $"api-log-{DateTime.UtcNow:yyyyMMdd}.jsonl");

            var json = JsonSerializer.Serialize(entry, _jsonOptions);

            await File.AppendAllTextAsync(file, json + Environment.NewLine);
        }
    }
}
