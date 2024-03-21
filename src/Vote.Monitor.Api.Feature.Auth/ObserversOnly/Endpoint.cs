using Authorization.Policies;

namespace Vote.Monitor.Api.Feature.Auth.ObserversOnly;

public class Endpoint: EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/api/auth/observersGreeting");
        Policies(PolicyNames.ObserversOnly);
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
    }

    public override async Task<string> ExecuteAsync(CancellationToken ct)
    {
        await Task.CompletedTask;

        return "Hello observer!";
    }
}
