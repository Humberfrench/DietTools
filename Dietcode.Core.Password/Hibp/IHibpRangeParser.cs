namespace Dietcode.Core.Password.Hibp;

public interface IHibpRangeParser
{
    /// <summary>
    /// Procura <paramref name="expectedSuffix"/> entre as linhas
    /// "SUFFIX:COUNT" de <paramref name="response"/>. Entradas de padding
    /// (COUNT = 0) são descartadas. Retorna 0 se não encontrado.
    /// </summary>
    long FindOccurrences(string response, string expectedSuffix);
}
