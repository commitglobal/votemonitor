using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validators;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.Validators;

public class SectionRequestValidator : Validator<SectionRequest>
{
    public SectionRequestValidator() : this([])
    {
    }

    public SectionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Title)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 3, 256));

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
        {
            v.Add<TextQuestionRequest>(new TextInputQuestionRequestValidator(languages));
            v.Add<NumberQuestionRequest>(new NumberInputQuestionRequestValidator(languages));
            v.Add<DateQuestionRequest>(new DateInputQuestionRequestValidator(languages));
            v.Add<SingleSelectQuestionRequest>(new SingleSelectQuestionRequestValidator(languages));
            v.Add<MultiSelectQuestionRequest>(new MultiSelectQuestionRequestValidator(languages));
            v.Add<RatingQuestionRequest>(new RatingQuestionRequestValidator(languages));
        });
    }
}
