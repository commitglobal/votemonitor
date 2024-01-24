using Vote.Monitor.Api.Feature.PollingStation.Helpers;
using Vote.Monitor.Api.Feature.PollingStation.Specifications;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.Feature.PollingStation.Create;
public class Endpoint : Endpoint<Request, Results<Ok<PollingStationModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<PollingStationAggregate> _repository;
    private readonly ITimeProvider _timeProvider;

    public Endpoint(IRepository<PollingStationAggregate> repository,
        ITimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public override void Configure()
    {
        Post("api/polling-stations");
    }

    public override async Task<Results<Ok<PollingStationModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetPollingStationSpecification(req.Address, req.Tags);
        var hasIdenticalPollingStation = await _repository.AnyAsync(specification, ct);

        if (hasIdenticalPollingStation)
        {
            AddError("A polling station with same address and tags exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var pollingStation = new PollingStationAggregate(req.Address, req.DisplayOrder, req.Tags.ToTagsObject(), _timeProvider);
        await _repository.AddAsync(pollingStation, ct);

        return TypedResults.Ok(new PollingStationModel
        {
            Id = pollingStation.Id,
            Address = pollingStation.Address,
            DisplayOrder = pollingStation.DisplayOrder,
            Tags = pollingStation.Tags.ToDictionary(),
        });
    }
}
