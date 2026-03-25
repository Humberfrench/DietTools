using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{

    public sealed class MinItemsAttribute : ValidationAttribute
    {
        private readonly int _min;

        public MinItemsAttribute(int min)
        {
            _min = min;
            ErrorMessage = $"A lista deve conter no mínimo {min} item(ns).";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is ICollection col)
                return col.Count >= _min ? ValidationResult.Success : new ValidationResult(ErrorMessage);

            if (value is IEnumerable enumerable)
            {
                int count = 0;
                foreach (var _ in enumerable)
                {
                    count++;
                    if (count >= _min) return ValidationResult.Success;
                }
                return new ValidationResult(ErrorMessage);
            }

            return new ValidationResult("O atributo MinItems deve ser usado em uma coleção.");
        }
    }

    public sealed class MaxItemsAttribute : ValidationAttribute
    {
        private readonly int _max;

        public MaxItemsAttribute(int max)
        {
            _max = max;
            ErrorMessage = $"A lista deve conter no máximo {max} item(ns).";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null) return ValidationResult.Success;

            if (value is ICollection col)
                return col.Count <= _max ? ValidationResult.Success : new ValidationResult(ErrorMessage);

            if (value is IEnumerable enumerable)
            {
                int count = 0;
                foreach (var _ in enumerable)
                {
                    count++;
                    if (count > _max) return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("O atributo MaxItems deve ser usado em uma coleção.");
        }
    }
}
