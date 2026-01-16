using Microsoft.AspNetCore.Http;
using System.Text;


//MIDDLEEWAREARE PARA LOG DE REQUISIÇÕES E RESPOSTAS HTTP
//Coloque depois de UseRouting() e antes de UseEndpoints()
namespace Dietcode.Api.Core.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _logPath;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logPath = Path.Combine(AppContext.BaseDirectory, "logs");

            if (!Directory.Exists(_logPath))
                Directory.CreateDirectory(_logPath);
        }

        public async Task Invoke(HttpContext context)
        {
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var url = $"{context.Request.Method} {context.Request.Path}";

            // -------------------------
            // REQUEST BODY
            // -------------------------
            context.Request.EnableBuffering();

            string requestBody = "post vazio";

            if (context.Request.ContentLength > 0)
            {
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    leaveOpen: true);

                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // -------------------------
            // RESPONSE BODY
            // -------------------------
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var statusCode = context.Response.StatusCode;

            await responseBody.CopyToAsync(originalBodyStream);

            // -------------------------
            // WRITE LOG
            // -------------------------
            var logFile = Path.Combine(_logPath, $"log-{DateTime.Now:yyyyMMdd}.txt");

            var log = $"""
        {dateTime}
        URL: {url}
        REQUEST:
        {requestBody}

        RESPONSE:
        {responseText}

        STATUS: {statusCode}
        ----------------------------------------
        """;

            await File.AppendAllTextAsync(logFile, log);
        }
    }
}
