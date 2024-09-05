namespace Feature.Forms.GetCitizenReportingFormsVersion;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
