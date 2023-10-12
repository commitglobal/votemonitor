using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetAllPollingStations;
internal class GetAllPollingStationsEndpoint : Endpoint<GetAllPollingStationsRequest, PaginationResponse<PollingStationModel>>
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

    public override async Task HandleAsync(GetAllPollingStationsRequest req, CancellationToken ct)
    {
        try
        {
            var pollingStations = await _repository.GetAll(page: 1, pagesize: 30);

            //var filterCriteria = JsonSerializer.Deserialize<Dictionary<string, string>>(req.Filter);

            //if (!string.IsNullOrWhiteSpace(req.Filter) && filterCriteria != null)
            //{
            //    foreach (var criteria in filterCriteria)
            //    {
            //        pollingStations = pollingStations.Where(ps => ps.Tags.Any(tag =>
            //            tag.Key.Equals(criteria.Key, StringComparison.OrdinalIgnoreCase) &&
            //            tag.Value.Equals(criteria.Value, StringComparison.OrdinalIgnoreCase)));
            //    }
            //}

            req.PageSize = Math.Min(req.PageSize, 100);

            var totalItems = pollingStations.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / req.PageSize);

            var pollingStationsToShow = pollingStations.ToList();
            //    .OrderBy(ps => ps.Id)
            //    .Skip((req.Page - 1) * req.PageSize)
            //    .Take(req.PageSize)
            //    .ToList();

            var response = new PaginationResponse<PollingStationModel>
            {
                Results = pollingStationsToShow,
                CurrentPage = req.Page,
                PageSize = req.PageSize,
                TotalItems = totalItems,
                TotalPges = totalPages
            };

            await SendAsync(response, cancellation: ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving Polling Stations ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
