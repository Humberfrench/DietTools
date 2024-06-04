using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dietcode.Core.Lib
{
    public static class GeracaoDeChaves
    {
        public static string GerarSenhaAleatoria(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVXYWZabcdefghijklmnopqrstuvxywz0123456789";
            //var chars = "0123456789";
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
            //var chars = "0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return Convert.ToInt64(result);
        }
    }
}
