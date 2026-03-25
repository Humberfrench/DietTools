using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public class BusinessDayValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            if (value is DateTime dt)
                return dt.IsDiaNaoUtil() ? new ValidationResult(ErrorMessage ?? "A data deve ser um dia útil.") : ValidationResult.Success;

            if (value is DateOnly d)
                return d.IsDiaNaoUtil() ? new ValidationResult(ErrorMessage ?? "A data deve ser um dia útil.") : ValidationResult.Success;

            return ValidationResult.Success;
        }
    }
}
