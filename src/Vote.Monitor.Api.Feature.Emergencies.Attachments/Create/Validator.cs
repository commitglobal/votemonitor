using Vote.Monitor.Core.Validators;

namespace Vote.Monitor.Api.Feature.Emergencies.Attachments.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.EmergencyId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Attachment)
            .NotNull()
            .NotEmpty()
            .FileSmallerThan(512 * 1024 * 1024); // 500 MB upload limit
    }
}
