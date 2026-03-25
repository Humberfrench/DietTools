using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        private readonly string _startProperty;
        private readonly string _endProperty;

        public DateRangeValidationAttribute(string startProperty, string endProperty)
        {
            _startProperty = startProperty;
            _endProperty = endProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            var startProp = context.ObjectType.GetProperty(_startProperty);
            var endProp = context.ObjectType.GetProperty(_endProperty);

            if (startProp is null || endProp is null)
                return new ValidationResult($"Propriedades '{_startProperty}'/'{_endProperty}' não encontradas.");

            var start = startProp.GetValue(context.ObjectInstance);
            var end = endProp.GetValue(context.ObjectInstance);

            if (start is null || end is null) return ValidationResult.Success;

            if (start is DateTime s && end is DateTime e)
            {
                if (s > e) return new ValidationResult(ErrorMessage ?? "Data inicial deve ser menor ou igual à data final.");
            }
            else if (start is DateOnly so && end is DateOnly eo)
            {
                if (so > eo) return new ValidationResult(ErrorMessage ?? "Data inicial deve ser menor ou igual à data final.");
            }

            return ValidationResult.Success;
        }
    }
}
