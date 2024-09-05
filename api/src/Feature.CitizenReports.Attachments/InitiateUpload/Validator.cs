namespace Feature.CitizenReports.Attachments.InitiateUpload;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CitizenReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.NumberOfUploadParts).GreaterThan(0);
    }
}
