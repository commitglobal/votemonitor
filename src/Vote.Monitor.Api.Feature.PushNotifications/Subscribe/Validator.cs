namespace Vote.Monitor.Api.Feature.PushNotifications.Subscribe;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}

