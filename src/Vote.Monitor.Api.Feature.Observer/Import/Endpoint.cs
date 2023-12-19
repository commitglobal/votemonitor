using Microsoft.Extensions.Logging;
using Vote.Monitor.Api.Feature.Observer.Services;
using Vote.Monitor.Api.Feature.Observer.Specifications;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;

namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Endpoint : Endpoint<Request, Results<NoContent, BadRequest<ImportValidationErrorModel>>>
{
    readonly IRepository<ObserverAggregate> _repository;
    private readonly IRepository<ImportValidationErrors> _errorRepo;
    private readonly ICsvParser<ObserverImportModel> _parser;
    private readonly ILogger<Endpoint> _logger;
    private readonly VoteMonitorContext _context;
    public Endpoint(VoteMonitorContext context,
        IRepository<ObserverAggregate> repository,
        IRepository<ImportValidationErrors> errorRepo,
        ICsvParser<ObserverImportModel> parser,
        ILogger<Endpoint> logger)
    {
        _repository = repository;
        _errorRepo = errorRepo;
        _parser = parser;
        _logger = logger;
        _context = context;
    }

    public override void Configure()
    {
        Post("/api/observers:import");
        DontAutoTag();
        Options(x => x.WithTags("observers"));
        AllowFileUploads();
    }

    public override async Task<Results<NoContent, BadRequest<ImportValidationErrorModel>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var parsingResult = _parser.Parse(req.File.OpenReadStream());
        if (parsingResult is ParsingResult<ObserverImportModel>.Fail failedResult)
        {

            string csv = CsvRowParsedHelpers<ObserverImportModel>.ConstructErrorFileContent(failedResult.Items);
            var errorSaved = await _errorRepo.AddAsync(new(ImportType.Observer, req.File.Name, csv, DateTime.Now), ct);
            return TypedResults.BadRequest(
                new ImportValidationErrorModel { Id = errorSaved.Id, Message = "The file contains errors! Please use the ID to get the file with the errors described inside." });

        }

        var importedRows = parsingResult as ParsingResult<ObserverImportModel>.Success;
        List<ObserverAggregate> observers = importedRows!
            .Items
            .Select(x => new ObserverAggregate(x.Name, x.Email, x.Password, x.PhoneNumber))
            .ToList();
        List<ObserverAggregate> observersToAdd = new();

        var logins = observers.Select(testc => testc.Login);
        var specification = new GetObserversByLoginsSpecification(logins);
        var c = (await _repository.ListAsync(specification, ct)).ToList();

        var duplicates = observers.Where(x => c.Any(y => y.Login == x.Login)).ToList();

        foreach (var obs in duplicates)
        {
            _logger.LogWarning("An Observer with email {obs.Login} already exists!", obs.Login);
        }
        observersToAdd = observers.Where(x => !c.Any(y => y.Login == x.Login)).ToList();

        if (observersToAdd.Count > 0) await _repository.AddRangeAsync(observersToAdd, ct);

        return TypedResults.NoContent();
    }
}
