namespace Feature.QuickReports.InitiateUpload;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.QuickReportId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NumberOfUploadParts).GreaterThan(0);
    }
}
