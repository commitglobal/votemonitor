using FluentValidation.Validators;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class UnsupportedLanguageValidator : PropertyValidator<TranslatedString, KeyValuePair<string, string>>
{
    private readonly List<string> _supportedLanguages;

    public UnsupportedLanguageValidator(List<string> supportedLanguages)
    {
        _supportedLanguages = supportedLanguages;
    }

    public override bool IsValid(FluentValidation.ValidationContext<TranslatedString> context, KeyValuePair<string, string> translation)
    {
        if (!_supportedLanguages.Contains(translation.Key))
        {
            context.MessageFormatter.AppendArgument("LanguageCode", translation.Key);
            context.MessageFormatter.AppendArgument("SupportedLanguages", string.Join(",", _supportedLanguages));

            return false;
        }

        return true;
    }

    public override string Name => "UnsupportedLanguage";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Supported languages [{SupportedLanguages}] do not include {LanguageCode}.";
}
