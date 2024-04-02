using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Api.Feature.Form.Create;

public class Endpoint(IRepository<FormAggregate> repository,
    IRepository<ElectionRound> electionRoundRepository,
    IRepository<MonitoringNgo> monitoringNgoRepository) : Endpoint<Request, Results<Ok<FormModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngo/{monitoringNgoId}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
