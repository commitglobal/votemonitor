using FluentValidation;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Validators;

public class PartiallyTranslatedStringValidator : Validator<TranslatedString>
{
    public PartiallyTranslatedStringValidator() : this([])
    {

    }

    public PartiallyTranslatedStringValidator(List<string> supportedLanguages)
    {
        Func<string, bool> isNotEmpty = str => !string.IsNullOrWhiteSpace(str);
        Func<KeyValuePair<string, string>, bool> isValidTranslation = translation => isNotEmpty(translation.Key);

        RuleFor(x => x)
            .Must(ts => ts.Any(isValidTranslation))
            .WithMessage("Provide at least one translation")
            .Must((translatedString, _, context) =>
            {
                foreach (var supportedLanguage in supportedLanguages)
                {
                    if (translatedString.Keys.Contains(supportedLanguage))
                    {
                        continue;
                    }

                    context.MessageFormatter.AppendArgument("LanguageCode", supportedLanguage);

                    return false;
                }

                return true;
            })
            .WithMessage(@"Missing translation placeholder for ""{LanguageCode}""");

        RuleForEach(x => x)
            .OverrideIndexer((_, _, element, _) => $@"[""{element.Key}""]")
            .SetValidator(new InvalidLanguageCodeFormatValidator())
            .SetValidator(new UnsupportedLanguageValidator(supportedLanguages));
    }
}
