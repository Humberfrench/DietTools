namespace Dietcode.Core.Jobs
{
    /// <summary>
    /// Pedido de início de job assíncrono genérico:
    /// - HandlerKey: identifica qual handler/método será executado
    /// - Payload: dados do método
    /// </summary>
    public sealed record AsyncStartRequest<TRequest>(string HandlerKey, TRequest Payload);
}
