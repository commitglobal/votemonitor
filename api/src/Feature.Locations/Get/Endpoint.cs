using Feature.Locations.Specifications;
using Vote.Monitor.Core.Helpers;

namespace Feature.Locations.Get;

public class Endpoint : Endpoint<Request, Results<Ok<LocationModel>, NotFound>>
{
    private readonly IReadRepository<LocationAggregate> _repository;

    public Endpoint(IReadRepository<LocationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/locations/{id}");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
    }

    public override async Task<Results<Ok<LocationModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var location = await _repository.FirstOrDefaultAsync(new GetLocationByIdSpecification(req.ElectionRoundId, req.Id), ct);

        if (location is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new LocationModel
        {
            Id = location.Id,
            Level1 = location.Level1,
            Level2 = location.Level2,
            Level3 = location.Level3,
            Level4 = location.Level4,
            Level5 = location.Level5,
            DisplayOrder = location.DisplayOrder,
            Tags = location.Tags.ToDictionary()
        });
    }
}
