namespace Feature.IssueReports.Attachments.AbortUpload;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.IssueReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UploadId).NotEmpty();
    }
}
