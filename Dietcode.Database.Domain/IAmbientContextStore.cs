namespace Dietcode.Database.Domain
{
    public interface IAmbientContextStore
    {
        bool TryGet<T>(string key, out T value);
        void Set(string key, object value);

        /// <summary>
        /// Cria um "escopo" lógico. Útil para Web (1 por request) e Worker (1 por execução).
        /// </summary>
        IDisposable BeginScope();
    }
}
