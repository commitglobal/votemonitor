namespace Feature.IssueReports.Notes.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.IssueReportId).NotEmpty();
    }
}
