using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

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
            v.Add<TextInputQuestionRequest>(new TextInputQuestionRequestValidator(languages));
            v.Add<NumberInputQuestionRequest>(new NumberInputQuestionRequestValidator(languages));
            v.Add<DateInputQuestionRequest>(new DateInputQuestionRequestValidator(languages));
            v.Add<SingleSelectQuestionRequest>(new SingleSelectQuestionRequestValidator(languages));
            v.Add<MultiSelectQuestionRequest>(new MultiSelectQuestionRequestValidator(languages));
            v.Add<RatingQuestionRequest>(new RatingQuestionRequestValidator(languages));
            v.Add<GridQuestionRequest>(new GridQuestionRequestValidator(languages));
        });
    }
}
