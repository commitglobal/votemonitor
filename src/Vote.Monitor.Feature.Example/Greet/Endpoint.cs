using FastEndpoints;

namespace Vote.Monitor.Feature.Example.Greet;

public class Endpoint : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/exaple1");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await SendAsync(new()
        {
            Greeting = $"Hello {req.Name}"
        }, cancellation: ct);
    }
}
