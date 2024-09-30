namespace Feature.IssueReports.Attachments.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.IssueReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
