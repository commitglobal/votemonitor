namespace Feature.CitizenReports.Notes.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CitizenReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
