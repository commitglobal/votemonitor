using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validators;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.Validators;

public class MultiSelectQuestionRequestValidator : Validator<MultiSelectQuestionRequest>
{
    public MultiSelectQuestionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.QuestionType).NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 3, 256));

        RuleFor(x => x.Helptext)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 3, 256))
            .When(x => x.Helptext != null);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleForEach(x => x.Options)
            .SetValidator(new SelectOptionRequestValidator(languages));

        RuleFor(x => x.Options)
            .Must(x =>
            {
                var groupedOptionIds = Enumerable.GroupBy(x, o => o.Id,
                        (id, group) => new { id, count = group.Count() });

                return groupedOptionIds.All(g => g.count == 1);
            })
            .When(x => x.Options.Any())
            .WithMessage("Duplicated id found");
    }
}
