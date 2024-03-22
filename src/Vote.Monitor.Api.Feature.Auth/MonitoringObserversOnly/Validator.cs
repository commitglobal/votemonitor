
namespace Vote.Monitor.Api.Feature.Auth.MonitoringObserversOnly;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
