namespace Dietcode.Core.Cep;

internal static class CepNormalizer
{
    public static string? Normalize(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            return null;

        var digits = new string(cep.Where(char.IsDigit).ToArray());

        return digits.Length == 8 ? digits : null;
    }
}
