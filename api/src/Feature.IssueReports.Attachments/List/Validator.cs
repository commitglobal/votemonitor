namespace Feature.IssueReports.Attachments.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.IssueReportId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
    }
}
