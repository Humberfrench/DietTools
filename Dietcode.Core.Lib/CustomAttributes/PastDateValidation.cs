using System;
using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public class PastDateValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime dateTime)
                return ValidationResult.Success; // Nada para validar

            // Agora a lógica está correta:
            // ❌ Se a data é no futuro → inválido
            // ✔ Se é hoje ou no passado → válido
            if (dateTime > DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "A data deve ser no passado.");
            }

            return ValidationResult.Success;
        }
    }
}
