namespace Dietcode.Core.Lib.Boletos;

/// <summary>
/// Algoritmos de dígito verificador, puros. Todos exigem apenas dígitos
/// <c>0</c>–<c>9</c> e lançam <see cref="ArgumentException"/> caso contrário —
/// o legado (<c>CalculaModulo10</c>) ignorava caracteres inválidos em silêncio.
/// </summary>
internal static class DigitoVerificadorHelper
{
    /// <summary>
    /// Módulo 10 (pesos 2 e 1 alternados, da direita para a esquerda; produtos ≥ 10
    /// somam seus algarismos). Mesmo algoritmo de <c>Validator.CalculaModulo10</c>.
    /// </summary>
    /// <returns>Dígito de 0 a 9.</returns>
    public static int Modulo10(string numero)
    {
        var soma = 0;
        var peso = 2;

        for (var i = ValidarDigitos(numero) - 1; i >= 0; i--)
        {
            var produto = (numero[i] - '0') * peso;
            soma += produto >= 10 ? produto - 9 : produto;
            peso = peso == 2 ? 1 : 2;
        }

        var dv = 10 - (soma % 10);
        return dv == 10 ? 0 : dv;
    }

    /// <summary>
    /// Módulo 11 do código de barras bancário (pesos 2 a 9 cíclicos, da direita para
    /// a esquerda). <c>11 − resto</c>; resultado 0, 10 ou 11 vira 1.
    /// </summary>
    /// <returns>Dígito de 1 a 9.</returns>
    public static int Modulo11Bancario(string numero)
    {
        var dv = 11 - (SomaPonderadaModulo11(numero) % 11);
        return dv is 0 or 10 or 11 ? 1 : dv;
    }

    /// <summary>
    /// Módulo 11 da arrecadação (pesos 2 a 9 cíclicos, da direita para a esquerda).
    /// Resto 0 ou 1 resulta em 0; nos demais casos <c>11 − resto</c>.
    /// </summary>
    /// <returns>Dígito de 0 a 9.</returns>
    public static int Modulo11Arrecadacao(string numero)
    {
        var resto = SomaPonderadaModulo11(numero) % 11;
        return resto is 0 or 1 ? 0 : 11 - resto;
    }

    /// <summary>
    /// DV de arrecadação conforme o indicador da posição 3: <c>6</c>/<c>7</c> usam
    /// Módulo 10; <c>8</c>/<c>9</c> usam Módulo 11.
    /// </summary>
    /// <returns>O dígito, ou <c>null</c> se o indicador não for 6, 7, 8 ou 9.</returns>
    public static int? DigitoArrecadacao(char indicador, string numero)
        => indicador switch
        {
            '6' or '7' => Modulo10(numero),
            '8' or '9' => Modulo11Arrecadacao(numero),
            _ => null
        };

    private static int SomaPonderadaModulo11(string numero)
    {
        var soma = 0;
        var peso = 2;

        for (var i = ValidarDigitos(numero) - 1; i >= 0; i--)
        {
            soma += (numero[i] - '0') * peso;
            peso = peso == 9 ? 2 : peso + 1;
        }

        return soma;
    }

    private static int ValidarDigitos(string numero)
    {
        ArgumentNullException.ThrowIfNull(numero);

        foreach (var c in numero)
        {
            if (c is < '0' or > '9')
                throw new ArgumentException("O número deve conter apenas dígitos de 0 a 9.", nameof(numero));
        }

        return numero.Length;
    }
}
