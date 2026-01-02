using System.Runtime.CompilerServices;
namespace Dietcode.Core.Lib.Passwords
{
    public static class PasswordExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PasswordStrengthResult AnalyzePassword(
            this ReadOnlySpan<char> password,
            int minimumLength = 8)
        {
            if (password.IsEmpty)
                return CreateEmptyResult();

            PasswordFlags flags = PasswordFlags.None;

            foreach (var c in password)
            {
                // 1️⃣ Espaço em branco (regra: proibido)
                if (char.IsWhiteSpace(c))
                {
                    flags |= PasswordFlags.Whitespace;
                    continue;
                }

                // 2️⃣ Unicode / acentos (regra: proibido)
                if (c > 0x7F) // fora da tabela ASCII
                {
                    flags |= PasswordFlags.NonAscii;
                    continue;
                }

                // 3️⃣ Classificação normal (ASCII only)
                if (c is >= 'A' and <= 'Z')
                    flags |= PasswordFlags.Upper;
                else if (c is >= 'a' and <= 'z')
                    flags |= PasswordFlags.Lower;
                else if (c is >= '0' and <= '9')
                    flags |= PasswordFlags.Digit;
                else
                    flags |= PasswordFlags.Symbol;
            }


            bool hasUpper = flags.HasFlag(PasswordFlags.Upper);
            bool hasLower = flags.HasFlag(PasswordFlags.Lower);
            bool hasDigit = flags.HasFlag(PasswordFlags.Digit);
            bool hasSymbol = flags.HasFlag(PasswordFlags.Symbol);

            bool hasWhitespace = flags.HasFlag(PasswordFlags.Whitespace);
            bool hasNonAscii = flags.HasFlag(PasswordFlags.NonAscii);

            if (hasWhitespace || hasNonAscii)
            {
                return new PasswordStrengthResult
                {
                    Length = password.Length,
                    HasUppercase = flags.HasFlag(PasswordFlags.Upper),
                    HasLowercase = flags.HasFlag(PasswordFlags.Lower),
                    HasDigit = flags.HasFlag(PasswordFlags.Digit),
                    HasSymbol = flags.HasFlag(PasswordFlags.Symbol),
                    Entropy = 0,
                    Level = PasswordStrengthLevel.Invalid,
                    MeetsMinimumRules = false
                };
            }

            double entropy = CalculateEntropy(
                password.Length,
                hasUpper,
                hasLower,
                hasDigit,
                hasSymbol);

            var level = MapEntropyToLevel(entropy);

            bool meetsRules =
                password.Length >= minimumLength &&
                hasUpper &&
                hasLower &&
                hasDigit &&
                hasSymbol;

            return new PasswordStrengthResult
            {
                Length = password.Length,
                HasUppercase = hasUpper,
                HasLowercase = hasLower,
                HasDigit = hasDigit,
                HasSymbol = hasSymbol,
                Entropy = entropy,
                Level = level,
                MeetsMinimumRules = meetsRules
            };
        }

        // ------------------------
        // Internals
        // ------------------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double CalculateEntropy(
            int length,
            bool upper,
            bool lower,
            bool digit,
            bool symbol)
        {
            int poolSize = 0;

            if (upper) poolSize += 26;
            if (lower) poolSize += 26;
            if (digit) poolSize += 10;
            if (symbol) poolSize += 32; // símbolos comuns

            if (poolSize == 0 || length == 0)
                return 0;

            return length * Math.Log2(poolSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static PasswordStrengthLevel MapEntropyToLevel(double entropy)
            => entropy switch
            {
                < 28 => PasswordStrengthLevel.VeryWeak,
                < 36 => PasswordStrengthLevel.Weak,
                < 60 => PasswordStrengthLevel.Medium,
                < 128 => PasswordStrengthLevel.Strong,
                _ => PasswordStrengthLevel.VeryStrong
            };

        private static PasswordStrengthResult CreateEmptyResult()
            => new()
            {
                Length = 0,
                HasUppercase = false,
                HasLowercase = false,
                HasDigit = false,
                HasSymbol = false,
                Entropy = 0,
                Level = PasswordStrengthLevel.VeryWeak,
                MeetsMinimumRules = false
            };
    }
}
