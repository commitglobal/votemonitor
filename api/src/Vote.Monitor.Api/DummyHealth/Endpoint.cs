namespace Vote.Monitor.Api.DummyHealth;

public class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync("healthy", cancellation: ct);
    }
}
