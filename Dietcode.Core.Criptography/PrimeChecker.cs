namespace Dietcode.Core.Criptography
{
    public class PrimeChecker
    {
        public bool IsPrime(int number)
        {
            // Números menores ou iguais a 1 não são primos
            if (number <= 1)
            {
                return false;
            }

            // Verifica se o número é divisível por qualquer número de 2 até a raiz quadrada do número
            for (int i = 2; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false; // O número não é primo
                }
            }

            return true; // O número é primo
        }
    }
}
