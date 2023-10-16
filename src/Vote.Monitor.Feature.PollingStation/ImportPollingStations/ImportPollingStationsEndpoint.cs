using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FastEndpoints;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
internal class ImportPollingStationsEndpoint : EndpointWithoutRequest
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<ImportPollingStationsEndpoint> _logger;

    public ImportPollingStationsEndpoint(IPollingStationRepository repository, ILogger<ImportPollingStationsEndpoint> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/polling-stations/import");

        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tempCSVPath = "C://temp";

        try
        {
            using (var reader = new StringReader(tempCSVPath))
            using (var csv = new CsvReader(reader, configuration: new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
            {
                var records = csv.GetRecords<PollingStationImport>().ToList();

                await _repository.DeleteAll();

                foreach (var record in records)
                {
                    var pollingStation = new PollingStationModel
                    {
                        DisplayOrder = record.DisplayOrder,
                        Address = record.Address,
                        Tags = record.Tags.ToTags()
                    };

                    await _repository.Add(pollingStation);
                }
                await SendAsync(records.Count, cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import Polling Stations ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();
    }
}
