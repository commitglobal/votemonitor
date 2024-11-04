using Feature.IncidentReports.Requests;

namespace Feature.IncidentReports.Validators;

public class IncidentReportsAggregateFilterValidator : Validator<IncidentReportsAggregateFilter>
{
    public IncidentReportsAggregateFilterValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
    }
}