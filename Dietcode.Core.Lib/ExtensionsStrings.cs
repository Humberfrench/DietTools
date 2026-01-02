using System.Text;

namespace Dietcode.Core.Lib
{
    public static partial class Extensions
    {
        /// <summary>
        /// Converte uma string Base64 para texto UTF-8.
        /// Retorna string vazia em caso de erro.
        /// </summary>
        public static string FromBase64ToString(this string stringBase64)
        {
            if (string.IsNullOrWhiteSpace(stringBase64))
                return string.Empty;

            try
            {
                byte[] data = Convert.FromBase64String(stringBase64);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
                // Mantém compatibilidade: não lança exceção
                return string.Empty;
            }
        }
        /// <summary>
        /// Trunca a string para no máximo o número de caracteres especificado.
        /// </summary>
        /// <param name="value">Texto de entrada</param>
        /// <param name="maxLength">Comprimento máximo permitido</param>
        /// <returns>Texto truncado ou original</returns>
        public static string Truncate(this string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= maxLength
                ? value
                : value.Substring(0, maxLength);
        }
    }
}
