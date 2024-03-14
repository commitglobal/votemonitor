using FastEndpoints;
using Vote.Monitor.Domain.Entities.FormBase.Validation;

namespace Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

public class PollingStationInfoFormValidator : Validator<PollingStationInfoForm>
{
    public PollingStationInfoFormValidator()
    {
        RuleForEach(x => x.Sections)
            .SetValidator(x => new SectionValidator());
    }
}
