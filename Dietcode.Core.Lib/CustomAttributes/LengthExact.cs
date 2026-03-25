using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public sealed class LengthExactAttribute : ValidationAttribute
    {
        private readonly int _length;

        public LengthExactAttribute(int length)
        {
            _length = length;
            ErrorMessage = $"O campo deve ter exatamente {length} caracteres.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            var s = value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(s)) return ValidationResult.Success;

            return s.Length == _length
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}
