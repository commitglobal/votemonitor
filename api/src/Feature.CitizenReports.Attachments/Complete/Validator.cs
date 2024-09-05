namespace Feature.CitizenReports.Attachments.Complete;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CitizenReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UploadId).NotEmpty();
        RuleFor(x => x.Etags).NotEmpty();
        RuleForEach(x => x.Etags).NotEmpty();
    }
}
