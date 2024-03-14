using FluentValidation.Validators;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Validators;

public class TranslationLengthValidator : PropertyValidator<TranslatedString, KeyValuePair<string, string>>
{
    private readonly int _maximumLength;
    private readonly int _minimumLength;

    public TranslationLengthValidator(int minimumLength, int maximumLength)
    {
        _maximumLength = maximumLength;
        _minimumLength = minimumLength;
    }

    public override bool IsValid(FluentValidation.ValidationContext<TranslatedString> context, KeyValuePair<string, string> translation)
    {
        bool IsNotEmpty(string str) => !string.IsNullOrWhiteSpace(str);
        bool HasValidTextLength(KeyValuePair<string, string> t) => !IsNotEmpty(t.Value) || t.Value.Length >= _minimumLength && t.Value.Length <= _maximumLength;

        if (!HasValidTextLength(translation))
        {
            context.MessageFormatter.AppendArgument("LanguageCode", translation.Key);
            context.MessageFormatter.AppendArgument("MinLength", _minimumLength);
            context.MessageFormatter.AppendArgument("MaxLength", _maximumLength);

            return false;
        }

        return true;
    }

    public override string Name => "InvalidTranslationLength";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Translation for '{LanguageCode}' must be between {MinLength} and {MaxLength} characters.";
}
