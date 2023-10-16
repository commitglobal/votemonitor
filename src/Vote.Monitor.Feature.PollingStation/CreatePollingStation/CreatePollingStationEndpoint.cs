using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class CreatePollingStationEndpoint : Endpoint<PollingStationCreateRequestDto, PollingStationReadDto, CreatePollingStationMapper>
{
    private readonly IPollingStationRepository _repository;

    public CreatePollingStationEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Post("api/polling-stations");

        AllowAnonymous();
    }

    public override async Task HandleAsync(PollingStationCreateRequestDto req, CancellationToken ct)
    {
        var model = Map.ToEntity(req);

        var result = await _repository.AddAsync(model);

        await SendCreatedAtAsync<GetPollingStationEndpoint>(result, Map.FromEntity(model));
    }

}
