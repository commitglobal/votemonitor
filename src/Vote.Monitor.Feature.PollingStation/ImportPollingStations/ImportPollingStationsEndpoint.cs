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
        int importedCount = 0;

        try
        {
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

                foreach (var record in records)
                {
                    var pollingStation = new PollingStationModel
                    {
                        DisplayOrder = record.DisplayOrder,
                        Address = record.Address,
                        Tags = record.Tags.ToTags()
                    };

                    await _repository.AddAsync(pollingStation);

                    importedCount++;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to import Polling Stations ");

            AddError(ex.Message);
        }

        ThrowIfAnyErrors();

        return importedCount;
    }
}
