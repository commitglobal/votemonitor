using Feature.Locations.Specifications;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;

namespace Feature.Locations.List;
public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<LocationModel>>, ProblemDetails>>
{
    private readonly IReadRepository<LocationAggregate> _repository;

    public Endpoint(IReadRepository<LocationAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/locations:list");
        DontAutoTag();
        Options(x => x.WithTags("locations"));
    }

    public override async Task<Results<Ok<PagedResponse<LocationModel>>, ProblemDetails>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var specification = new ListLocationsSpecification(request);
        var locations = await _repository.ListAsync(specification, ct);
        var locationsCount = await _repository.CountAsync(specification, ct);
        var result = locations.Select(x => new LocationModel
        {
            Id = x.Id,
            Level1 = x.Level1,
            Level2 = x.Level2,
            Level3 = x.Level3,
            Level4 = x.Level4,
            Level5 = x.Level5,
            DisplayOrder = x.DisplayOrder,
            Tags = x.Tags.ToDictionary()
        }).ToList();

        return TypedResults.Ok(new PagedResponse<LocationModel>(result, locationsCount, request.PageNumber, request.PageSize));
    }
}
