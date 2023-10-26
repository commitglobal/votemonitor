using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Vote.Monitor.Core;

namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal partial class GetAllPollingStationsEndpoint : Endpoint<GetAllPollingStationsRequest, PaginationResponse<PollingStationReadDto>, GetAllPollingStationsMapper>
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<GetAllPollingStationsEndpoint> _logger;

    public GetAllPollingStationsEndpoint(IPollingStationRepository repository, ILogger<GetAllPollingStationsEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {


        Get("/api/polling-stations");

        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllPollingStationsRequest request, CancellationToken ct)
    {
        List<TagModel>? filterCriteria = null;
        if (request.Filter != null) filterCriteria = TagModelExtensions.DecodeFilter(request.Filter);

        int totalItems = await _repository.CountAsync(filterCriteria: filterCriteria);
        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        List<PollingStationModel> pollingStations = (await _repository.GetAllAsync(filterCriteria: filterCriteria, request.PageSize, request.Page)).ToList();

        PaginationResponse<PollingStationReadDto> response = Map.FromEntity(pollingStations, request.Page, request.PageSize, totalItems, totalPages);

        await SendAsync(response, cancellation: ct);

    }
}
