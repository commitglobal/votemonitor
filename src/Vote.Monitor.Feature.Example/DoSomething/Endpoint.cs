using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Vote.Monitor.Feature.Example.Options;
using Vote.Monitor.Feature.Example.Services;

namespace Vote.Monitor.Feature.Example.DoSomething;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ISomethingSomethingService _service;
    private readonly UsefulOptions _options;

    public Endpoint(ISomethingSomethingService service, IOptions<UsefulOptions> options)
    {
        _service = service;
        _options = options.Value;
    }

    public override void Configure()
    {
        Post("/api/something");
        DontAutoTag();
        Description(x => x.WithTags("Example"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _service.DoSomethingAsync(req.Parameter);

        switch (result)
        {
            case SomethingResult.Ok r:
                await SendAsync(new Response { Message = r.Result }, cancellation: ct);
                break;

            case SomethingResult.Error e:
                foreach (var error in e.Errors)
                {
                    AddError(error);
                }

                ThrowIfAnyErrors();
                break;
        }
    }
}
