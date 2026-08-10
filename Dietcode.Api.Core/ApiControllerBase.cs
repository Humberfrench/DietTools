using Dietcode.Api.Core.Attributes;
using Dietcode.Api.Core.Middleware;
using Dietcode.Api.Core.Results;
using Dietcode.Api.Core.Results.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ResultStatus = Dietcode.Api.Core.Results;

namespace Dietcode.Api.Core
{

    public abstract class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase() { }

        // ---------------------------------------------------------
        // HOOK DE EXTENSÃO (override se quiser enriquecer/logar)
        // ---------------------------------------------------------
        [NonAction]
        protected virtual MethodResult BeforeReturn(MethodResult result)
        {
            // Aqui você pode:
            // - logar
            // - auditar
            // - enriquecer erros
            // - mapear códigos customizados 600+ para HTTP padrão, etc.
            return result;
        }

        // ---------------------------------------------------------
        // COMPLETED POR STATUS SIMPLES
        // ---------------------------------------------------------
        [NonAction]
        protected IActionResult Completed(ResultStatusCode statusCode)
        {
            return CreateStatusCodeResult(statusCode);
        }


        // ---------------------------------------------------------
        // COMPLETED (ponto único de tradução MethodResult -> IActionResult)
        // O overload genérico existe só para facilitar inferência de tipo
        // no call site; toda a lógica vive na versão não genérica abaixo,
        // então Created/NotFound/Location-header se comportam igual
        // independente de qual overload o controller chamar.
        // ---------------------------------------------------------

        [NonAction]
        protected IActionResult Completed<TContent>(MethodResult<TContent> result)
        {
            return Completed((MethodResult)result);
        }

        // ---------------------------------------------------------
        // COMPLETED SEM MethodResult (para serviços que não usam AppServiceBase)
        // Reaproveita o mesmo pipeline acima (ProblemDetails, code, Location
        // header quando aplicável, ShouldReturnNotFound, etc.) — só monta o
        // MethodResult internamente a partir do (content, status) recebido.
        // ---------------------------------------------------------

        [NonAction]
        protected IActionResult Completed<TContent>(TContent content, ResultStatusCode statusCode)
        {
            if ((int)statusCode >= StatusCodes.Status400BadRequest)
            {
                // Mesmo espírito do fallback em Completed(MethodResult): tenta aproveitar
                // o que foi passado como descrição do erro; sem nada reconhecível, cai no
                // genérico -- nunca lança, sempre resolve para um ProblemDetails coerente.
                return content switch
                {
                    IEnumerable<ErrorValidation> errors => CreateErrorResponse(statusCode, errors),
                    ErrorValidation error => CreateErrorResponse(statusCode, new[] { error }),
                    string message => CreateErrorResponse(statusCode, new[] { new ErrorValidation(statusCode.ToString(), message) }),
                    _ => CreateErrorResponse(
                        new ErrorResult(statusCode, new ErrorValidation(statusCode.ToString(), "Erro não especificado.")))
                };
            }

            return Completed(new StatusContentResult<TContent>(content, statusCode));
        }

        [NonAction]
        protected IActionResult Completed(ResultStatusCode statusCode, string message)
            => Completed(statusCode, new ErrorValidation(null!, message));

        [NonAction]
        protected IActionResult Completed(ResultStatusCode statusCode, ErrorValidation error)
            => Completed(new ErrorResult(statusCode, error));

        [NonAction]
        protected IActionResult Completed(ResultStatusCode statusCode, IEnumerable<ErrorValidation> errors)
            => Completed(new ErrorResult(statusCode, errors));

        [NonAction]
        protected IActionResult Completed(MethodResult result)
        {
            result = BeforeReturn(result);

            if ((int)result.Status >= StatusCodes.Status400BadRequest)
            {
                if (result is IErrorResult errorResult)
                {
                    return CreateErrorResponse(result.Status, errorResult.Errors);
                }

                return CreateErrorResponse(
                    new ErrorResult(result.Status, new ErrorValidation(result.Status.ToString(),
                                                                        "Erro não especificado.")));
            }

            if (result is IContentResult contentResult)
            {
                if (contentResult.Status == ResultStatusCode.Created && result is ICreatedResult createdResult)
                {
                    return CompletedAtAction(createdResult, "Get");
                }

                if (contentResult.Status == ResultStatusCode.OK && ShouldReturnNotFound(contentResult.Content))
                {
                    return Completed(new ResultStatus.NotFoundResult(
                        new ErrorValidation("404", "Nenhum registro encontrado.")));
                }

                var content = contentResult.Content ?? new { };
                return CreateObjectResult(contentResult.Status, content);
            }

            return CreateStatusCodeResult(result.Status);
        }

        [NonAction]
        protected IActionResult Canceled(string? message = null)
        {
            var error = new ErrorValidation("499", message ?? "Requisição cancelada pelo cliente.");
            return Completed(new ClientClosedResult(error));
        }

        [NonAction]
        protected IActionResult Canceled<TContent>(TContent content, string? message = null)
        {
            var error = new ErrorValidation("499", message ?? "Requisição cancelada pelo cliente.");
            return Completed(new ClientClosedResult<TContent>(content, error));
        }

        // ---------------------------------------------------------
        // CREATED AT ACTION (para CreatedResult<T>)
        // ---------------------------------------------------------
        private IActionResult CompletedAtAction(ICreatedResult createdResult, string actionName)
        {
            var location = Url.Action(
                action: actionName,
                controller: null,                         // mesmo controller
                values: new { id = createdResult.Identifier },
                protocol: Request.Scheme);

            return Created(location!, createdResult.Content);
        }

        // ---------------------------------------------------------
        // ERROS → PROBLEMDETAILS / VALIDATIONPROBLEMDETAILS
        // ---------------------------------------------------------
        private IActionResult CreateErrorResponse(ErrorResult errorResult)
        {
            var errors = errorResult.Errors?.ToArray() ?? Array.Empty<ErrorValidation>();

            // Se tiver mais de um erro, usamos ValidationProblemDetails
            if (errors.Length > 1)
            {
                var vpd = CreateValidationProblemDetails(errorResult);
                return new ObjectResult(vpd)
                {
                    StatusCode = vpd.Status
                };
            }

            // Um erro só → ProblemDetails simples
            var pd = CreateProblemDetails(errorResult);
            return new ObjectResult(pd)
            {
                StatusCode = pd.Status
            };
        }
        private IActionResult CreateErrorResponse(ResultStatusCode status, IEnumerable<ErrorValidation> errors)
        {
            var list = errors?.ToArray() ?? Array.Empty<ErrorValidation>();

            if (list.Length > 1)
            {
                var vpd = CreateValidationProblemDetails(new ErrorResult(status, list));
                return new ObjectResult(vpd) { StatusCode = vpd.Status };
            }

            var pd = CreateProblemDetails(new ErrorResult(status, list.Length == 1 ? list[0] : new ErrorValidation(status.ToString(), "Erro não especificado.")));
            return new ObjectResult(pd) { StatusCode = pd.Status };
        }


        [NonAction]
        protected virtual ProblemDetails CreateProblemDetails(ErrorResult errorResult, string? instanceOverride = null)
        {
            var first = errorResult.Errors?.FirstOrDefault();
            var message = first?.Message ?? "Ocorreu um erro. Favor acionar o suporte.";

            var details = new ProblemDetails
            {
                Status = (int)errorResult.Status,
                Detail = message,
                Title = GetTitleFromStatus(errorResult.Status),
                Type = GetTypeFromStatus(errorResult.Status),
                Instance = instanceOverride ?? HttpContext?.Request?.Path
            };

            // Enriquecimento padrão
            details.Extensions["traceId"] = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
            details.Extensions["timestamp"] = DateTimeOffset.UtcNow;
            details.Extensions["code"] = string.IsNullOrWhiteSpace(first?.Code) ? null : first!.Code;

            return details;
        }

        [NonAction]
        protected virtual ValidationProblemDetails CreateValidationProblemDetails(ErrorResult errorResult, string? instanceOverride = null)
        {
            // Agrupa por Code (quando informado pelo ErrorBuilder/enum) para o
            // consumidor conseguir tratar programaticamente; sem Code, cai no
            // bucket "General" como antes.
            var errorsDict = (errorResult.Errors ?? Enumerable.Empty<ErrorValidation>())
                .Where(e => !string.IsNullOrWhiteSpace(e.Message))
                .GroupBy(e => string.IsNullOrWhiteSpace(e.Code) ? "General" : e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());

            if (errorsDict.Count == 0)
                errorsDict["General"] = new[] { "Erro de validação." };

            var vpd = new ValidationProblemDetails(errorsDict)
            {
                Status = (int)errorResult.Status,
                Title = GetTitleFromStatus(errorResult.Status),
                Type = GetTypeFromStatus(errorResult.Status),
                Detail = "Uma ou mais validações falharam.",
                Instance = instanceOverride ?? HttpContext?.Request?.Path
            };

            vpd.Extensions["traceId"] = HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();
            vpd.Extensions["timestamp"] = DateTimeOffset.UtcNow;

            return vpd;
        }

        // ---------------------------------------------------------
        // HELPERS DE TITLE/TYPE PARA PROBLEMDETAILS
        // ---------------------------------------------------------
        private static string GetTitleFromStatus(ResultStatusCode status)
        {
            return status switch
            {
                ResultStatusCode.BadRequest => "Requisição inválida",
                ResultStatusCode.Unauthorized => "Não autorizado",
                ResultStatusCode.Forbidden => "Proibido",
                ResultStatusCode.NotFound => "Não encontrado",
                ResultStatusCode.MethodNotAllowed => "Método não permitido",
                ResultStatusCode.NotAcceptable => "Não aceitável",
                ResultStatusCode.TimeOut => "Tempo excedido",
                ResultStatusCode.Conflict => "Conflito",
                ResultStatusCode.UnsupportedMediaType => "Tipo de conteúdo não suportado",
                ResultStatusCode.UnprocessableEntity => "Entidade não processável",
                ResultStatusCode.PreconditionFailed => "Precondição falhou",
                ResultStatusCode.PreconditionRequired => "Precondição obrigatória",
                ResultStatusCode.InternalServerError => "Erro interno no servidor",
                ResultStatusCode.BadGateway => "Falha em dependência externa",
                ResultStatusCode.ServiceUnavailable => "Serviço indisponível",
                ResultStatusCode.GatewayTimeout => "Tempo excedido em dependência externa",
                _ => "Erro"
            };
        }

        private static string GetTypeFromStatus(ResultStatusCode status)
        {
            // Se quiser, pode apontar para uma página de documentação por código
            // Ex: https://httpstatuses.com/404
            return $"https://httpstatuses.com/{(int)status}";
        }

        // ---------------------------------------------------------
        // OBJECT / STATUS HELPERS
        // ---------------------------------------------------------
        private ObjectResult CreateObjectResult(ResultStatusCode statusCode, object content)
        {
            var objResult = new ObjectResult(content)
            {
                StatusCode = (int)statusCode
            };

            objResult.ContentTypes.Add(new MediaTypeHeaderValue("application/json"));
            return objResult;
        }

        private StatusCodeResult CreateStatusCodeResult(ResultStatusCode statusCode)
            => new StatusCodeResult((int)statusCode);

        // ---------------------------------------------------------
        // ReturnValue (compatibilidade com código legado)
        // ---------------------------------------------------------
        [NonAction]
        protected IActionResult ReturnValue(MethodResult retorno, string instance)
        {
            if (retorno is ErrorResult errorResult)
            {
                // Reusa a lógica de ProblemDetails, mas com instance customizada
                var problem = CreateProblemDetails(errorResult, instance);

                return retorno.Status switch
                {
                    ResultStatusCode.Unauthorized => Unauthorized(problem),
                    ResultStatusCode.NotFound => NotFound(problem),
                    ResultStatusCode.BadRequest => BadRequest(problem),
                    _ => StatusCode(problem.Status ?? 500, problem)
                };
            }

            // Se não for erro, delega para Completed padrão
            return Completed(retorno);
        }

        // ---------------------------------------------------------
        // ProblemsDetails Returns (helper adicional)
        // ---------------------------------------------------------
        [NonAction]
        protected ProblemDetails ObterErro(string title, int status, string detail, string instance)
        {
            var problem = new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = detail,
                Instance = instance
            };

            problem.Extensions.Add("TraceId", Guid.NewGuid().ToString());

            return problem;
        }

        protected bool CheckRateLimit(out IActionResult? rateLimitResult)
        {
            rateLimitResult = null;

            // Endpoint atual
            var endpoint = HttpContext.GetEndpoint();
            var attribute = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();

            // Não tem RateLimit → OK
            if (attribute == null)
                return false;

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var cacheKey = $"rl:{endpoint!.DisplayName}:{ip}";

            var rateLimiter =
                HttpContext.RequestServices.GetRequiredService<IRateLimiter>();

            var result = rateLimiter.Check(
                cacheKey,
                attribute.Limit,
                TimeSpan.FromSeconds(attribute.Seconds));

            if (result.IsLimited)
            {
                // Header padrão HTTP
                HttpContext.Response.Headers["Retry-After"] =
                    ((int)result.RetryAfter.TotalSeconds).ToString();

                var payload = new ErrorValidation(
                    "429",
                    $"Muitas solicitações. Tente novamente em {Math.Ceiling(result.RetryAfter.TotalSeconds)} segundos.");

                rateLimitResult = Completed(new TooManyRequestsResult(payload));
                return true;
            }

            return false;
        }
        private static bool ShouldReturnNotFound(object? content)
        {
            if (content is null)
                return true;

            if (content is string)
                return false;

            if (content is System.Collections.IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                try
                {
                    return !enumerator.MoveNext();
                }
                finally
                {
                    (enumerator as IDisposable)?.Dispose();
                }
            }

            return false;
        }

    }
}