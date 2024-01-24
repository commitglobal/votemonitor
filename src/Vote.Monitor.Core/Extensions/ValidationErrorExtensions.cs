using FluentValidation.Results;

namespace Vote.Monitor.Core.Extensions;

public static class ValidationErrorExtensions
{
    public static IDictionary<string, string[]> ToValidationErrorDictionary(this List<ValidationFailure> errors)
    {
        return errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
    }
}
