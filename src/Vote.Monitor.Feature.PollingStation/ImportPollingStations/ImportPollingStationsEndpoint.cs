using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FastEndpoints;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
internal class ImportPollingStationsEndpoint : EndpointWithoutRequest
{
    private readonly IPollingStationRepository _repository;
    public ImportPollingStationsEndpoint(IPollingStationRepository repository)
    {
        _repository = repository;
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

                await _repository.DeleteAllAsync();

                foreach (var record in records)
                {
                    var pollingStation = new PollingStationModel
                    {
                        DisplayOrder = record.DisplayOrder,
                        Address = record.Address,
                        Tags = record.Tags.ToTags()
                    };

                    await _repository.AddAsync(pollingStation);
                }
                await SendAsync(records.Count, cancellation: ct);
            }
        }
        catch (Exception ex)
        {
            AddError("Failed to import polling stations." + ex.Message);
        }
    }
}
