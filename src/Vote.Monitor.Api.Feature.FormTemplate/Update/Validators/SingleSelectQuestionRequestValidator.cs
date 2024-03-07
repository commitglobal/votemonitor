﻿using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class SingleSelectQuestionRequestValidator : Validator<SingleSelectQuestionRequest>
{
    public SingleSelectQuestionRequestValidator(List<string> languages)
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
            .Must(options =>
            {
                var groupedOptionIds = options
                    .GroupBy(o => o.Id, (id, group) => new { id, count = group.Count() });

                return groupedOptionIds.All(g => g.count == 1);
            })
            .WithMessage("Duplicated id found");
    }
}
