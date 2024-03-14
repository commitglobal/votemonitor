using FastEndpoints;
using Vote.Monitor.Core.Validation;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormBase.Validation;

internal class SelectOptionValidator : Validator<SelectOption>
{
    internal SelectOptionValidator()
    {
        RuleFor(x => x.Text)
            .SetValidator(new TranslatedStringValidator());
    }
}
