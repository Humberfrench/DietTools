// =============================================================
// 3) IHandlerDispatcher.cs (dispatcher por HandlerKey)
// Namespace: Dietcode.Core.Jobs.Interfaces
// =============================================================
namespace Dietcode.Core.Jobs.Interfaces
{
    /// <summary>
    /// Dispatcher resolve e executa um handler pelo HandlerKey.
    /// Ele é o coração do "executar qualquer método".
    /// </summary>
    public interface IHandlerDispatcher
    {
        /// <summary>
        /// Executa o handler identificado por handlerKey usando o payloadJson.
        /// Retorna o resultado já serializado em JSON.
        /// </summary>
        Task<string> ExecuteAsync(string handlerKey, string payloadJson, CancellationToken ct);
    }
}
