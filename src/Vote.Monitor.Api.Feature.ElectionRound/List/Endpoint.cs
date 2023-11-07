using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.ElectionRound.List;

public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<ElectionRoundModel>>, ProblemDetails>>
{
     readonly IReadRepository<ElectionRoundAggregate> _repository;

    public Endpoint(IReadRepository<ElectionRoundAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds");
    }

    public override async Task<Results<Ok<PagedResponse<ElectionRoundModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
