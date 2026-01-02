namespace Dietcode.Core.Lib.Passwords
{
    public sealed class PasswordStrengthResult
    {
        public required int Length { get; init; }
        public required bool HasUppercase { get; init; }
        public required bool HasLowercase { get; init; }
        public required bool HasDigit { get; init; }
        public required bool HasSymbol { get; init; }

        public required double Entropy { get; init; }
        public required PasswordStrengthLevel Level { get; init; }

        public bool MeetsMinimumRules { get; init; }
    }
}
