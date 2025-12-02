using System;
using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public class FutureDateValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime dateTime)
                return ValidationResult.Success; // nada para validar

            // Lógica correta:
            // ✔ Se a data for no futuro → válido
            // ❌ Se for hoje ou no passado → inválido
            if (dateTime <= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "A data deve ser no futuro.");
            }

            return ValidationResult.Success;
        }
    }
}
