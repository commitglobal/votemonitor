using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Domain.Entities.FormBase.Validation;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public class PollingStationInfoFormValidator : Validator<PollingStationInformationForm>
{
    public PollingStationInfoFormValidator()
    {
        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add(new TextQuestionValidator());
                v.Add(new NumberQuestionValidator());
                v.Add(new DateQuestionValidator());
                v.Add(new SingleSelectQuestionValidator());
                v.Add(new MultiSelectQuestionValidator());
                v.Add(new RatingQuestionValidator());
            });
    }
}
