namespace Dietcode.Core.Lib
{
    public static class Util
    {
        public static string GerarSenhaAleatoria(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVXYWZabcdefghijklmnopqrstuvxywz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return result;
        }
        public static long GerarNumeroUnico(int tamanho)
        {
            var chars = "0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return Convert.ToInt64(result);
        }
        public static string GerarReferenceId(int tamanho, string prefixo)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVXYWZabcdefghijklmnopqrstuvxywz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return $"{prefixo}-{result}";
        }

    }
}
