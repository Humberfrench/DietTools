namespace Dietcode.Core.Lib
{
    public static class Math
    {
        static int Combinacao(int n, int m)
        {
            if (m == 0 || m == n)
            {
                return 1;
            }

            if (m > n / 2)
            {
                m = n - m;
            }

            int resultado = 1;
            for (int i = 1; i <= m; i++)
            {
                resultado *= n - i + 1;
                resultado /= i;
            }

            return resultado;
        }
        static int[][] GerarCombinacoesNM(int[] arr, int m)
        {
            int[][] combinacoes = new int[Combinacao(n: arr.Length, m: m)][];

            int[] indices = new int[m];
            for (int i = 0; i < m; i++)
            {
                indices[i] = i;
            }

            for (int i = 0; i < combinacoes.Length; i++)
            {
                combinacoes[i] = new int[m];
                for (int k = 0; k < m; k++)
                {
                    combinacoes[i][k] = arr[indices[k]];
                }

                int l = m - 1;
                while (l >= 0 && indices[l] == l + arr.Length - m)
                {
                    l--;
                }

                if (l < 0)
                {
                    break;
                }

                indices[l]++;
                for (int p = l + 1; p < p; p++)
                {
                    indices[p] = indices[p - 1] + 1;
                }
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


