using Dietcode.Api.Core.Results;
using Dietcode.Api.Core.Results.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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
        // COMPLETED GENÉRICO (para métodos que retornam TContent)
        // ---------------------------------------------------------
        [NonAction]
        protected IActionResult Completed<TContent>(MethodResult result)
        {
            result = BeforeReturn(result);

            // 1) Se tiver conteúdo tipado (sucesso ou erro)
            if (result is IContentResult<TContent> contentResult)
            {
                // Se for Created<T>, gera Location
                if (contentResult.Status == ResultStatusCode.Created &&
                    result is CreatedResult<TContent> createdResult)
                {
                    return CompletedAtAction(createdResult, "Get");
                }

                return CreateObjectResult(contentResult.Status, contentResult.Content!);
            }

            // 2) Se for erro sem payload tipado
            if (result is ErrorResult errorResult)
            {
                return CreateErrorResponse(errorResult);
            }

            // 3) Só status (NoContent, etc.)
            return CreateStatusCodeResult(result.Status);
        }

        // ---------------------------------------------------------
        // COMPLETED NÃO GENÉRICO (para quem não sabe o tipo)
        // ---------------------------------------------------------
        [NonAction]
        protected IActionResult Completed(MethodResult result)
        {
            result = BeforeReturn(result);

            // 1) Conteúdo não tipado (IContentResult simples)
            if (result is IContentResult contentResult)
            {
                var content = contentResult.Content ?? new { };
                return CreateObjectResult(contentResult.Status, content);
            }

            // 2) Erro sem payload explícito
            if (result is ErrorResult errorResult)
            {
                return CreateErrorResponse(errorResult);
            }

            // 3) Apenas status
            return CreateStatusCodeResult(result.Status);
        }

        // ---------------------------------------------------------
        // CREATED AT ACTION (para CreatedResult<T>)
        // ---------------------------------------------------------
        private IActionResult CompletedAtAction<TContent>(CreatedResult<TContent> createdResult, string actionName)
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

            return details;
        }

        [NonAction]
        protected virtual ValidationProblemDetails CreateValidationProblemDetails(ErrorResult errorResult, string? instanceOverride = null)
        {
            var messages = errorResult.Errors?
                .Select(e => e.Message)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray() ?? Array.Empty<string>();

            // Como não sabemos o campo de cada erro, agrupamos em uma categoria geral
            var errorsDict = new System.Collections.Generic.Dictionary<string, string[]>
            {
                ["General"] = messages.Length > 0 ? messages : new[] { "Erro de validação." }
            };

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
                ResultStatusCode.NotAcceptable => "Não aceitável",
                ResultStatusCode.TimeOut => "Tempo excedido",
                ResultStatusCode.Conflict => "Conflito",
                ResultStatusCode.UnprocessableEntity => "Entidade não processável",
                ResultStatusCode.InternalServerError => "Erro interno no servidor",
                ResultStatusCode.ServiceUnavailable => "Serviço indisponível",
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
    }
}