namespace Dietcode.Classic.Lib.Passwords
{
    public sealed class PasswordStrengthResult
    {
        public int Length { get; set; }
        public bool HasUppercase { get; set; }
        public bool HasLowercase { get; set; }
        public bool HasDigit { get; set; }
        public bool HasSymbol { get; set; }

        public double Entropy { get; set; }
        public PasswordStrengthLevel Level { get; set; }

        public bool MeetsMinimumRules { get; set; }
    }
}

