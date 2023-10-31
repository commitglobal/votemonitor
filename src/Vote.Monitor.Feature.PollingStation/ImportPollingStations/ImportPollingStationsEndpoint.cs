using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using FastEndpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
public class ImportPollingStationsEndpoint : EndpointWithoutRequest
{
    private readonly IPollingStationRepository _repository;
    private readonly ILogger<ImportPollingStationsEndpoint> _logger;
    private readonly IConfiguration _configuration;
    public ImportPollingStationsEndpoint(IPollingStationRepository repository, ILogger<ImportPollingStationsEndpoint> logger, IConfiguration config)
    {
        _repository = repository;
        _logger = logger;
        _configuration = config;
    }

    public override void Configure()
    {
        Post("/api/polling-stations/import");

        AllowAnonymous();
    }

    public override async Task<int> HandleAsync(CancellationToken ct)
    {
        var csvFilePath = _configuration.GetSection("CSVFileToImport")["path"];

        int rowsImported = 0;

        if (!File.Exists(csvFilePath))
        {
            throw new NotFoundException<ImportPollingStationsEndpoint>($"CSV file not found at path: {csvFilePath}");
        }

        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, configuration: new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        }))
        {
            csv.Context.RegisterClassMap<PollingStationImportDTOMap>();

            var records = csv.GetRecords<PollingStationImport>().ToList();

            await _repository.DeleteAllAsync();

            var validRecords = new List<PollingStationModel>();

            foreach (var record in records)
            {
                var pollingStation = new PollingStationModel
                {
                    DisplayOrder = record.DisplayOrder,
                    Address = record.Address,
                    Tags = record.Tags.ToTags()
                };

                if (IsValid(pollingStation))
                    validRecords.Add(pollingStation);
            }

            rowsImported = await _repository.BulkInsertAsync(validRecords);

            if (rowsImported <= 0) throw new Exception("No polling station was inserted");
        }
        return rowsImported;
    }

    private bool IsValid(PollingStationModel pollingStation)
    {
        if (pollingStation.DisplayOrder < 0)
            return false;

        if (string.IsNullOrWhiteSpace(pollingStation.Address))
            return false;

        if (!pollingStation.Tags.Any())
            return false;

        return true;
    }
}
