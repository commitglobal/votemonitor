using Vote.Monitor.Core.Services.Parser;

namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Endpoint(
    IRepository<ObserverAggregate> repository,
    IRepository<ImportValidationErrors> errorRepo,
    ICsvParser<ObserverImportModel> parser,
    ILogger<Endpoint> logger)
    : Endpoint<Request, Results<NoContent, BadRequest<ImportValidationErrorModel>>>
{
    public override void Configure()
    {
        Post("/api/observers:import");
        DontAutoTag();
        Options(x => x.WithTags("observers"));
        AllowFileUploads();
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, BadRequest<ImportValidationErrorModel>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var parsingResult = parser.Parse(req.File.OpenReadStream());
        if (parsingResult is ParsingResult<ObserverImportModel>.Fail failedResult)
        {

            string csv = failedResult.Items.ConstructErrorFileContent();
            var errorSaved = await errorRepo.AddAsync(new(ImportType.Observer, req.File.Name, csv), ct);
            return TypedResults.BadRequest(
                new ImportValidationErrorModel { Id = errorSaved.Id, Message = "The file contains errors! Please use the ID to get the file with the errors described inside." });
        }

        var importedRows = parsingResult as ParsingResult<ObserverImportModel>.Success;
        List<ObserverAggregate> observers = importedRows!
            .Items
            .Select(x => ObserverAggregate.Create(x.Name, x.Email, x.Password, x.PhoneNumber))
            .ToList();

        var logins = observers.Select(o => o.Login);
        var specification = new GetObserversByLoginsSpecification(logins);
        var existingObservers = await repository.ListAsync(specification, ct);

        var duplicates = observers.Where(x => existingObservers.Any(y => y.Login == x.Login)).ToList();

        foreach (var obs in duplicates)
        {
            logger.LogWarning("An Observer with email {obs.Login} already exists!", obs.Login);
        }
        List<ObserverAggregate> observersToAdd = observers.Where(x => !existingObservers.Any(y => y.Login == x.Login)).ToList();

        if (observersToAdd.Count > 0) await repository.AddRangeAsync(observersToAdd, ct);

        return TypedResults.NoContent();
    }
}
