using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.Feature.PollingStation.Specifications;

namespace Vote.Monitor.Feature.PollingStation.List;
internal class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>>
{
    private readonly IReadRepository<Domain.Entities.PollingStationAggregate.PollingStation> _repository;

    public Endpoint(IReadRepository<Domain.Entities.PollingStationAggregate.PollingStation> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations");
        RequestBinder(new RequestBinder());
    }

    public override async Task<Results<Ok<PagedResponse<PollingStationModel>>, ProblemDetails>> ExecuteAsync(Request request, CancellationToken ct)
    {
        var specification = new ListPollingStationsSpecification(request.AddressFilter, request.TagFilter, request.PageSize, request.PageNumber);
        var csos = await _repository.ListAsync(specification, ct);
        var csosCount = await _repository.CountAsync(specification, ct);
        var result = csos.Select(x => new PollingStationModel
        {
            Id = x.Id,
            Address = x.Address,
            DisplayOrder = x.DisplayOrder,
            Tags = x.Tags.ToDictionary()
        }).ToList();

        return TypedResults.Ok(new PagedResponse<PollingStationModel>(result, csosCount, request.PageNumber, request.PageSize));
    }
}
