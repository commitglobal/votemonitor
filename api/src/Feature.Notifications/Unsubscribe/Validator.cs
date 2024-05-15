namespace Feature.Notifications.Unsubscribe;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ObserverId).NotEmpty();
    }
}
