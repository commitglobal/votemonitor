using EFCore.BulkExtensions;
using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Vote.Monitor.Core.Services.Csv;
using Vote.Monitor.Domain;
using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;

namespace Vote.Monitor.Feature.PollingStation.Import;
public class Endpoint : Endpoint<Request, Results<Ok<Response>, NotFound, ProblemDetails>>
{
    private readonly VoteMonitorContext _context;
    private readonly ICsvReader<PollingStationImportModel> _reader;
    private const int MAX_INVALID_ROWS = 100;
    private readonly PollingStationImportModelValidator _pollingStationImportModelValidator = new();

    public Endpoint(VoteMonitorContext context, ICsvReader<PollingStationImportModel> reader)
    {
        _context = context;
        _reader = reader;
    }

    public override void Configure()
    {
        Post("/api/polling-stations/import");
        AllowFileUploads();
    }

    public override async Task<Results<Ok<Response>, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var pollingStations = _reader.ReadAsync<ImportModelMapper>(req.File.OpenReadStream(), ct);
        int numberOfInvalidRows = 0;
        int rowCount = 0;

        var entities = new List<PollingStationAggregate>();

        await foreach (var pollingStation in pollingStations)
        {
            var validationResult = Validate(pollingStation, rowCount);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.ForEach(AddError);

                ++numberOfInvalidRows;

                if (numberOfInvalidRows > MAX_INVALID_ROWS)
                {
                    break;
                }
            }

            var tags = pollingStation.Tags.ToDictionary(k => k.Name, v => v.Value).ToTagsObject();
            var entity = new PollingStationAggregate(pollingStation.Address, pollingStation.DisplayOrder, tags);
            entities.Add(entity);

            rowCount++;
        }

        ThrowIfAnyErrors();

        await _context.PollingStations.BatchDeleteAsync(cancellationToken: ct);
        await _context.BulkInsertAsync(entities, cancellationToken: ct);
        await _context.BulkSaveChangesAsync(cancellationToken: ct);

        return TypedResults.Ok(new Response { RowsImported = rowCount });
    }

    private ValidationResult Validate(PollingStationImportModel pollingStation, int rowIndex)
    {
        var validationContext = new FluentValidation.ValidationContext<PollingStationImportModel>(pollingStation)
        {
            RootContextData =
            {
                ["RowIndex"] = rowIndex
            }
        };

        return _pollingStationImportModelValidator.Validate(validationContext);
    }
}
