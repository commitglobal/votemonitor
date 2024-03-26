using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validation;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormBase.Validation;

internal class MultiSelectQuestionValidator : Validator<MultiSelectQuestion>
{
    internal MultiSelectQuestionValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Text)
            .SetValidator(new TranslatedStringValidator());

        RuleFor(x => x.Helptext)
            .SetValidator(new TranslatedStringValidator())
            .When(x => x.Helptext != null);

        RuleForEach(x => x.Options)
            .SetValidator(new SelectOptionValidator());
    }
}
