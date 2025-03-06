namespace Feature.Attachments.InitiateUpload;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.NumberOfUploadParts).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();

        RuleFor(x => x.LastUpdatedAt)
            .Must(BeUtc)
            .WithMessage("LastUpdatedAt must be in UTC format.");
    }

    private bool BeUtc(DateTime? date)
    {
        if (!date.HasValue)
        {
            return true;
        }

        return date.Value.Kind == DateTimeKind.Utc;
    }
}
