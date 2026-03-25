using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public sealed class OnlyDigitsAttribute : ValidationAttribute
    {
        public OnlyDigitsAttribute()
        {
            ErrorMessage = "O campo deve conter apenas números.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            var s = value.ToString();
            if (string.IsNullOrWhiteSpace(s)) return ValidationResult.Success;

            foreach (var ch in s!)
            {
                if (!char.IsDigit(ch))
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
