using Authorization.Policies;

namespace Vote.Monitor.Api.Feature.Auth.PlatformAdminsOnly;

public class Endpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/api/auth/platformAdminsGreeting");
        Policies(PolicyNames.PlatformAdminsOnly);
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
    }

    public override async Task<string> ExecuteAsync(CancellationToken ct)
    {
        await Task.CompletedTask;

        return "Hello platform admin!";
    }
}
