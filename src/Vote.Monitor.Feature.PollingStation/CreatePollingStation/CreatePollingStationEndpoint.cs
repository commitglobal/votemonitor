using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Feature.PollingStation.GetPollingStation;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class CreatePollingStationEndpoint : Endpoint<PollingStationCreateRequestDto, PollingStationReadDto, CreatePollingStationMapper>
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<CreatePollingStationEndpoint> _logger;
    public CreatePollingStationEndpoint(IPollingStationRepository repository, ILogger<CreatePollingStationEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("api/polling-stations");

        AllowAnonymous();
    }

    public override async Task HandleAsync(PollingStationCreateRequestDto req, CancellationToken ct)
    {
        try
        {
            var model = Map.ToEntity(req);

            var result = await _repository.Add(model);

            await SendCreatedAtAsync<GetPollingStationEndpoint>(result, Map.FromEntity(model));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Polling Station ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }

}
