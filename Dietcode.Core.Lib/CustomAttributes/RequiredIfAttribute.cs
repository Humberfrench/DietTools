using System.ComponentModel.DataAnnotations;

namespace Dietcode.Core.Lib.CustomAttributes
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _expectedValue;

        public RequiredIfAttribute(string propertyName, object expectedValue)
        {
            _propertyName = propertyName;
            _expectedValue = expectedValue;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_propertyName);
            if (property == null)
                return new ValidationResult($"Propriedade {_propertyName} não encontrada.");

            var propertyValue = property.GetValue(validationContext.ObjectInstance);

            if (Equals(propertyValue, _expectedValue))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
