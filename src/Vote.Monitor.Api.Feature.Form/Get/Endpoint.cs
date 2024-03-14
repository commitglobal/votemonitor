
using Vote.Monitor.Form.Module.Models;

namespace Vote.Monitor.Api.Feature.Form.Get;

public class Endpoint(IReadRepository<FormAggregate> repository) : Endpoint<Request, Results<Ok<FormModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
