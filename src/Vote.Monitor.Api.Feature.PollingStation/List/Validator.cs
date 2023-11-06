namespace Vote.Monitor.Api.Feature.PollingStation.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.PageNumber).GreaterThan(0);

        RuleFor(x => x.Filter)
            .Must(filter =>
            {
                return filter!.Keys.All(tag => !string.IsNullOrWhiteSpace(tag));
            })
            .When(x => x.Filter != null && x.Filter.Any());
    }
}
