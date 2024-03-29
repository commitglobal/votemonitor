﻿using Vote.Monitor.Core.Validators;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Api.Feature.Form.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.MonitoringNgoId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages)
            .NotNull()
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.FormType)
            .NotEmpty();

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add(x => new TextQuestionRequestValidator(x.Languages));
                v.Add(x => new NumberQuestionRequestValidator(x.Languages));
                v.Add(x => new DateQuestionRequestValidator(x.Languages));
                v.Add(x => new SingleSelectQuestionRequestValidator(x.Languages));
                v.Add(x => new MultiSelectQuestionRequestValidator(x.Languages));
                v.Add(x => new RatingQuestionRequestValidator(x.Languages));
            });
    }
}
