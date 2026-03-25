using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{

    public sealed class NotWhiteSpaceAttribute : ValidationAttribute
    {
        public NotWhiteSpaceAttribute()
        {
            ErrorMessage = "O campo não pode conter apenas espaços.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success; // deixe [Required] cuidar de null, se quiser

            if (value is string s && string.IsNullOrWhiteSpace(s))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
