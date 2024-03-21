using Authorization.Policies;

namespace Vote.Monitor.Api.Feature.Auth.NgoAdminsOnly;

public class Endpoint: EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/api/auth/ngoAdminsGreeting");
        Policies(PolicyNames.NgoAdminsOnly);
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
    }

    public override async Task<string> ExecuteAsync(CancellationToken ct)
    {
        await Task.CompletedTask;

        return "Hello ngo admin!";
    }
}
