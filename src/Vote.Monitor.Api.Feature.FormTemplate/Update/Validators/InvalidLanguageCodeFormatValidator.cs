using FluentValidation.Validators;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class InvalidLanguageCodeFormatValidator : PropertyValidator<TranslatedString, KeyValuePair<string, string>>
{
    public override bool IsValid(FluentValidation.ValidationContext<TranslatedString> context, KeyValuePair<string, string> translation)
    {
        if (string.IsNullOrWhiteSpace(translation.Key) || translation.Key.Length != 2)
        {
            context.MessageFormatter.AppendArgument("LanguageCode", translation.Key);
            return false;
        }

        return true;
    }

    public override string Name => "InvalidLanguageCodeFormat";

    protected override string GetDefaultMessageTemplate(string errorCode)
        => @"Code ""{LanguageCode}"" is in invalid format.";
}
