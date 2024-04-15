using Authorization.Policies;

namespace Vote.Monitor.Api.Feature.Auth.NgoAdminsOnly;

public class Endpoint() : Endpoint<Request, Results<Ok<string>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/auth/ngoAdminsGreeting");
        DontAutoTag();
        Options(x => x.WithTags("test-auth-policies"));
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<string>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        return TypedResults.Ok("Hello ngo admin!");
    }
}
