using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validation;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Domain.Entities.FormBase.Validation;

internal class TimeQuestionValidator : Validator<TimeQuestion>
{
    internal TimeQuestionValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Text)
            .SetValidator(new TranslatedStringValidator());
    }
}
