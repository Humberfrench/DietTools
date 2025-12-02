using System.Security.Cryptography;

namespace Dietcode.Core.Lib
{
    public static class Util
    {
        private const string LettersAndDigits = "ABCDEFGHIJKLMNOPQRSTUVXYWZabcdefghijklmnopqrstuvxywz0123456789";
        private const string Digits = "0123456789";

        /// <summary>
        /// Gera uma senha aleatória usando RNG criptográfico.
        /// </summary>
        public static string GerarSenhaAleatoria(int tamanho)
        {
            return GenerateRandomString(LettersAndDigits, tamanho);
        }

        /// <summary>
        /// Gera um número único com exatamente N dígitos.
        /// OBS: retorna string para preservar zeros à esquerda.
        /// Se você quiser manter long, ainda será possível converter.
        /// </summary>
        public static string GerarNumeroUnico(int tamanho)
        {
            return GenerateRandomString(Digits, tamanho);
        }

        /// <summary>
        /// Gera um ReferenceId com prefixo + "-" + random.
        /// </summary>
        public static string GerarReferenceId(int tamanho, string prefixo)
        {
            var randomPart = GenerateRandomString(LettersAndDigits, tamanho);
            return $"{prefixo}-{randomPart}";
        }

        /// <summary>
        /// Função interna genérica para montar strings seguras.
        /// </summary>
        private static string GenerateRandomString(string alphabet, int size)
        {
            var buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                int index = RandomNumberGenerator.GetInt32(alphabet.Length);
                buffer[i] = alphabet[index];
            }

            return new string(buffer);
        }
    }
}
