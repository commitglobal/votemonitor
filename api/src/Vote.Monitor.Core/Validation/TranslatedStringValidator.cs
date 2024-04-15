using FluentValidation;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Core.Validation;

public class TranslatedStringValidator : Validator<TranslatedString>
{
    public TranslatedStringValidator()
    {
        RuleForEach(x => x)
            .OverrideIndexer((_, _, element, _) => $@"[""{element.Key}""]")
            .Must(translation => !string.IsNullOrWhiteSpace(translation.Value))
            .WithMessage("Missing translation");
    }
}
