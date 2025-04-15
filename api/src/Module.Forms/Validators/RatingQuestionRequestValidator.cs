using FastEndpoints;
using FluentValidation;
using Module.Forms.Requests;
using Vote.Monitor.Core.Validators;

namespace Module.Forms.Validators;

public class RatingQuestionRequestValidator : Validator<RatingQuestionRequest>
{
    public RatingQuestionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.QuestionType).NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages));

        RuleFor(x => x.Helptext)
            .SetValidator(new PartiallyTranslatedStringValidator(languages))
            .When(x => x.Helptext != null);

        RuleFor(x => x.UpperLabel)
            .SetValidator(new PartiallyTranslatedStringValidator(languages))
            .When(x => x.UpperLabel != null);

        RuleFor(x => x.LowerLabel)
            .SetValidator(new PartiallyTranslatedStringValidator(languages))
            .When(x => x.LowerLabel != null);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Scale).NotEmpty();

        RuleFor(x => x.DisplayLogic)
            .SetValidator(new DisplayLogicRequestValidator());
    }
}