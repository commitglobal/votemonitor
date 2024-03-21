
namespace Vote.Monitor.Api.Feature.Auth.MonitoringNgoAdminsOnly;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
