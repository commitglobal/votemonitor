namespace Vote.Monitor.Api.Feature.PollingStation.Import;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.File)
            .NotEmpty();

        RuleFor(x => x.File)
            .Must(file => Path.GetExtension(file.FileName).ToLower() == ".csv")
            .When(x => x.File != null)
            .WithMessage("Only CSV files are accepted.");
    }
}
