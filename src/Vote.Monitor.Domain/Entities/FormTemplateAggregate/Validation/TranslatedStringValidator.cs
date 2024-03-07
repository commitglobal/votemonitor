using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Validation;

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