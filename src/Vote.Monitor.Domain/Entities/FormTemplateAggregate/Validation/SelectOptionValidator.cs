using FastEndpoints;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Validation;

public class SelectOptionValidator : Validator<SelectOption>
{
    public SelectOptionValidator()
    {
        RuleFor(x => x.Text)
            .SetValidator(new TranslatedStringValidator());
    }
}