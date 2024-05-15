using Vote.Monitor.Core.Validators;

namespace Feature.QuickReports.AddAttachment;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.QuickReportId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Attachment)
            .NotNull()
            .NotEmpty()
            .FileSmallerThan(512 * 1024 * 1024); // 500 MB upload limit
    }
}
