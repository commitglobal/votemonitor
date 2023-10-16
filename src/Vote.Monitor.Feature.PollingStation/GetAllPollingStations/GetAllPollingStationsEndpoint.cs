using FastEndpoints;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Vote.Monitor.Core;

namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal partial class GetAllPollingStationsEndpoint : Endpoint<GetAllPollingStationsRequest, PaginationResponse<PollingStationReadDto>, GetAllPollingStationsMapper>
{//GetAllPollingStationsRequest,
    private readonly IPollingStationRepository _repository;
    public GetAllPollingStationsEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {


        Get("/api/polling-stations");///{pagesize:int}{page:int}{filter:Dictionary<string,string>}");

        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllPollingStationsRequest request , CancellationToken ct)
    {
        Dictionary<string, string>? filterCriteria = null;
        if (request.Filter!=null) filterCriteria=FilterDecoder.DecodeFilter(request.Filter);

        int totalItems = await _repository.CountAsync(filterCriteria: filterCriteria);
        int totalPages = (int)Math.Ceiling((double)totalItems / request.PageSize);

        List<PollingStationModel> pollingStations = (await _repository.GetAllAsync(filterCriteria: filterCriteria, request.PageSize, request.Page)).ToList();      
     
        PaginationResponse<PollingStationReadDto> response = Map.FromEntity(pollingStations, request.Page, request.PageSize, totalItems,totalPages);

        await SendAsync(response);
    }
}
