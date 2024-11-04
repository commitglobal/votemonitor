using Feature.CitizenReports.Requests;

namespace Feature.CitizenReports.Validators;

public class CitizenReportsAggregateFilterValidator : Validator<CitizenReportsAggregateFilter>
{
    public CitizenReportsAggregateFilterValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
    }
}