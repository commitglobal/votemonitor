namespace Vote.Monitor.Api.Feature.PollingStation.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Address)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Tags)
            .Must(filter =>
            {
                return filter.Keys.All(tag => !string.IsNullOrWhiteSpace(tag));
            })
            .When(x => x.Tags != null && x.Tags.Any());
    }
}
