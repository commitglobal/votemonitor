namespace Feature.Citizen.Notifications.ListReceived;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
