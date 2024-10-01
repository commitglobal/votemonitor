namespace Feature.IncidentsReports.Attachments.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.IncidentReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
    }
}
