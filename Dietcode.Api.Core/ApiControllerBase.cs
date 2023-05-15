using Dietcode.Api.Core.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Linq;

namespace Dietcode.Api.Core
{
    public abstract class ApiControllerBase : ControllerBase
    {
        public ApiControllerBase()
        {

        }

        [NonAction]
        protected IActionResult Completed(ResultStatusCode statusCode)
        {
            return CreateStatusCodeResult(statusCode);
        }

        [NonAction]
        protected IActionResult Completed<TContent>(MethodResult methodResult)
        {
            var contentResult = methodResult as MethodResult<TContent>;
            var errorResult = methodResult as ErrorResult;

            if (methodResult.Status == ResultStatusCode.Created)
                return CompletedAtAction<TContent>(methodResult, "Get");

            if (contentResult != null)
                return CreateObjectResult(contentResult.Status, contentResult.Content ?? new object());

            if (errorResult != null)
            {
                var error = errorResult.Errors?.FirstOrDefault();

                if (error != null)
                    return CreateObjectResult(errorResult.Status, error);

                return CreateStatusCodeResult(errorResult.Status);
            }

            return CreateStatusCodeResult(methodResult.Status);
        }

        [NonAction]
        protected IActionResult Completed(MethodResult methodResult)
        {
            var errorResult = methodResult as ErrorResult;

            if (errorResult == null)
                return CreateStatusCodeResult(methodResult.Status);

            var error = errorResult.Errors?.FirstOrDefault();

            if (error != null)
                return CreateObjectResult(errorResult.Status, error);

            return CreateStatusCodeResult(errorResult.Status);
        }

        private IActionResult CompletedAtAction<TContent>(MethodResult methodResult, string actionName)
        {
            var createdResult = methodResult as CreatedResult<TContent>;

            var location = $"{Request.Scheme}://{Request.Host.ToUriComponent()}{Url.Action(actionName)}/{createdResult.Identifier}";

            return Created(location, createdResult.Content);
        }

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
        {
            return new StatusCodeResult((int)statusCode);
        }

        [NonAction]
        protected IActionResult ReturnValue(MethodResult retorno, string instance)
        {
            var errorResult = retorno as ErrorResult;
            var erro = "Ocorreu um erro. Favor acionar o suporte.";

            if (errorResult != null)
            {
                var error = errorResult.Errors?.FirstOrDefault();
                if (error != null)
                {
                    erro = error.Message;
                }
            }

            var retornoErro = new ProblemDetails
            {
                Status = (int)retorno.Status,
                Detail = erro,
                Title = "Erro.",
                Type = "Erro.",
                Instance = instance
            };

            if (retorno.Status == ResultStatusCode.Unauthorized)
            {
                retornoErro.Type = "Autorização";
                return Unauthorized(retornoErro);
            }
            else if (retorno.Status == ResultStatusCode.NotFound)
            {
                return NotFound(retornoErro);
            }
            else
            {
                return BadRequest(retornoErro);
            }
        }
    }
}