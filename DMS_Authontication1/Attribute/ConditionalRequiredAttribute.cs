using System;
using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ConditionalRequiredAttribute : ValidationAttribute
{
    private readonly string _dependentProperty;
    private readonly object _targetValue;

    public ConditionalRequiredAttribute(string dependentProperty, object targetValue)
    {
        _dependentProperty = dependentProperty;
        _targetValue = targetValue;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var dependentPropertyValue = validationContext.ObjectInstance.GetType()
            .GetProperty(_dependentProperty)
            .GetValue(validationContext.ObjectInstance);

        if (dependentPropertyValue != null && dependentPropertyValue.Equals(_targetValue))
        {
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                return new ValidationResult(ErrorMessage ?? "This field is required.");
            }
        }

        return ValidationResult.Success;
    }
}