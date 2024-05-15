namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.File)
            .NotEmpty();

        RuleFor(x => x.File)
            .Must(file => file.Length < 25 * 1024 * 1024) // 25 MB
            .When(x => x.File != null)
            .WithMessage("The selected file exceeds 25 MB limit.");

        RuleFor(x => x.File)
            .Must(file => Path.GetExtension(file.FileName).ToLower() == ".csv")
            .When(x => x.File != null)
            .WithMessage("Only CSV files are accepted.");
    }
}
