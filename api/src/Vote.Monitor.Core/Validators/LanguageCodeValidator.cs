using FluentValidation.Validators;
using Vote.Monitor.Core.Constants;

namespace Vote.Monitor.Core.Validators;

public class LanguageCodeValidator<T> : PropertyValidator<T, string>
{
    public override bool IsValid(FluentValidation.ValidationContext<T> context, string value)
    {
        return !string.IsNullOrWhiteSpace(value) && LanguagesList.IsKnownLanguage(value);
    }

    public override string Name => "LanguageCodeValidator";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Provided language code is not valid.";
}
