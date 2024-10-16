using Vote.Monitor.Core.Validators;

namespace Feature.CitizenReports.SendCopy;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.CitizenReportId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}