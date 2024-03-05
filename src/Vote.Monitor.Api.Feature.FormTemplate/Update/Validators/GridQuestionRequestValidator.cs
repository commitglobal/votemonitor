using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class GridQuestionRequestValidator : Validator<GridQuestionRequest>
{
    public GridQuestionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.QuestionType).NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 3, 256));

        RuleFor(x => x.Helptext)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 3, 256))
            .When(x => x.Helptext != null);

        RuleFor(x => x.Scale).NotEmpty();

        RuleFor(x => x.ScalePlaceholder)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 3, 256))
            .When(x => x.ScalePlaceholder != null);

        RuleForEach(x => x.Rows)
            .SetValidator(new GridQuestionRowRequestValidator(languages));
    }
}
