using Authorization.Policies;
using Feature.MonitoringObservers.Services;

namespace Feature.MonitoringObservers.Add;

public class Endpoint(IObserverImportService importService) : Endpoint<Request>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-observers");
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Creates new monitoring observers"; });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await importService.ImportAsync(req.ElectionRoundId, req.NgoId, req.Observers, ct);
        await SendNoContentAsync(ct);
    }
}