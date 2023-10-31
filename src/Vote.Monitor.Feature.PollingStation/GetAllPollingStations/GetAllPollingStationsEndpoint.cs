using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Vote.Monitor.Core;
using Vote.Monitor.Feature.PollingStation.RequestBinders;

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
        RequestBinder(new GetAllPollingStationsRequestBinder());
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllPollingStationsRequest request, CancellationToken ct)
    {
        int totalItems = await _repository.CountAsync(request.Filter);
        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        var pollingStations = (await _repository.GetAllAsync(request.Filter, request.PageSize, request.Page)).ToList();

        PaginationResponse<PollingStationReadDto> response = Map.FromEntity(pollingStations, request.PageSize, request.Page, totalItems, totalPages);

        await SendAsync(response, cancellation: ct);

    }
}
