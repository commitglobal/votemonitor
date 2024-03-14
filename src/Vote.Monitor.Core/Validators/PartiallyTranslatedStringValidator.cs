using FluentValidation;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Validators;

public class PartiallyTranslatedStringValidator : Validator<TranslatedString>
{
    public PartiallyTranslatedStringValidator() : this([], 3, 256)
    {

    }

    public PartiallyTranslatedStringValidator(List<string> supportedLanguages, int minimumLength, int maximumLength)
    {
        Func<string, bool> isNotEmpty = str => !string.IsNullOrWhiteSpace(str);
        Func<KeyValuePair<string, string>, bool> isValidTranslation = translation
            => isNotEmpty(translation.Value) && isNotEmpty(translation.Key);

        RuleFor(x => x)
            .Must(ts => Enumerable.Any(ts, isValidTranslation))
            .WithMessage("Provide at least one translation")
            .Must((ts, _, context) =>
            {
                foreach (var supportedLanguage in supportedLanguages)
                {
                    if (ts.Keys.Contains(supportedLanguage))
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
            .SetValidator(new TranslationLengthValidator(minimumLength, maximumLength))
            .SetValidator(new UnsupportedLanguageValidator(supportedLanguages));
    }
}
