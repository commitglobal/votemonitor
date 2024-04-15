
namespace Vote.Monitor.Api.Feature.Auth.MonitoringNgoAdminOrObserver;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
