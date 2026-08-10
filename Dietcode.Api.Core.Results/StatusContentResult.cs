using Dietcode.Api.Core.Results.Interfaces;

namespace Dietcode.Api.Core.Results
{
    /// <summary>
    /// Wrapper mínimo de (conteúdo, status) para quem não quer depender de
    /// AppServiceBase/OkResult/CreatedResult etc. no serviço.
    /// </summary>
    public class StatusContentResult<T> : MethodResult<T>, IContentResult<T>
    {
        public StatusContentResult(T content, ResultStatusCode status)
            : base(content, status)
        {
        }

        object IContentResult.Content => Content!;
    }
}
