namespace Dietcode.Core.Lib
{
    public record Combination(List<int> Valores);

    // renomeado internamente para evitar conflito com System.Math, mas compatível externamente
    public static class Math
    {
        public static List<Combination> GerarCombinacoesNM_Records(int[] arr, int m)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            if (m < 0 || m > arr.Length)
                throw new ArgumentException("m deve ser >= 0 && <= tamanho do array.");

            int total = Combinacao(arr.Length, m);
            var combinacoes = new List<Combination>(capacity: total);

            int[] indices = new int[m];
            for (int i = 0; i < m; i++)
                indices[i] = i;

            for (int i = 0; i < total; i++)
            {
                var combo = new List<int>(m);
                for (int k = 0; k < m; k++)
                    combo.Add(arr[indices[k]]);

                combinacoes.Add(new Combination(combo));

                int l = m - 1;
                while (l >= 0 && indices[l] == l + arr.Length - m)
                    l--;

                if (l < 0)
                    break;

                indices[l]++;

                for (int p = l + 1; p < m; p++)
                    indices[p] = indices[p - 1] + 1;
            }

            return combinacoes;
        }

        /// <summary>
        /// Calcula a combinação "n choose m" (nCm)
        /// </summary>
        public static int Combinacao(int n, int m)
        {
            if (m < 0 || n < 0 || m > n)
                throw new ArgumentException("Valores inválidos para combinação.");

            if (m == 0 || m == n)
                return 1;

            // combinação é simétrica, reduzimos para performance
            if (m > n / 2)
                m = n - m;

            int resultado = 1;
            for (int i = 1; i <= m; i++)
            {
                checked
                {
                    resultado *= (n - i + 1);
                    resultado /= i;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Gera todas as combinações possíveis de tamanho M a partir do array fornecido.
        /// </summary>
        public static int[][] GerarCombinacoesNM(int[] arr, int m)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            if (m < 0 || m > arr.Length)
                throw new ArgumentException("m deve ser >= 0 e <= tamanho do array.");

            int total = Combinacao(arr.Length, m);
            int[][] combinacoes = new int[total][];

            int[] indices = new int[m];
            for (int i = 0; i < m; i++)
                indices[i] = i;

            for (int i = 0; i < total; i++)
            {
                combinacoes[i] = new int[m];

                for (int k = 0; k < m; k++)
                    combinacoes[i][k] = arr[indices[k]];

                // encontra posição para incrementar
                int l = m - 1;
                while (l >= 0 && indices[l] == l + arr.Length - m)
                    l--;

                if (l < 0)
                    break;

                indices[l]++;

                // corrigido: montar sequência crescente
                for (int p = l + 1; p < m; p++)
                    indices[p] = indices[p - 1] + 1;
            }

            return combinacoes;
        }
    }
}


//static void Main()
//{
//    int[] arr = { 1, 2, 3, 4, 5 };
//    int n = arr.Length;
//    int m = 3;

//    int[][] combinacoes = GerarCombinacoesNM(arr, m);

//    for (int i = 0; i < combinacoes.Length; i++)
//    {
//        for (int j = 0; j < m; j++)
//        {
//            Console.Write(combinacoes[i][j] + " ");
//        }
//        Console.WriteLine();
//    }
//}
