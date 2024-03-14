using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validation;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormBase.Validation;

internal class NumberQuestionValidator : Validator<NumberQuestion>
{
    internal NumberQuestionValidator()
    {
        RuleFor(x => x.Text)
            .SetValidator(new TranslatedStringValidator());

        RuleFor(x => x.Helptext)
            .SetValidator(new TranslatedStringValidator())
            .When(x => x.Helptext != null);

        RuleFor(x => x.InputPlaceholder)
            .SetValidator(new TranslatedStringValidator())
            .When(x => x.InputPlaceholder != null);
    }
}
