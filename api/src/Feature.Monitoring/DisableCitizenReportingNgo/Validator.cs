namespace Feature.Monitoring.DisableCitizenReportingNgo;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();
    }
}
