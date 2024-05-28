using Vote.Monitor.Core.Validators;

namespace Feature.Attachments.AbortUpload;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UploadId).NotEmpty();
    }
}
