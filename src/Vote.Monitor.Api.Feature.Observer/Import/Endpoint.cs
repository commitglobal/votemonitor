using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Api.Feature.Observer.Services;
using Vote.Monitor.Api.Feature.Observer.Specifications;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;

namespace Vote.Monitor.Api.Feature.Observer.Import;

public class Endpoint : Endpoint<Request, Results<NoContent, BadRequest<ImportValidationErrorModel>>>
{
    readonly IRepository<ObserverAggregate> _repository;
    private readonly IRepository<ImportValidationErrors> _errorRepo;
    private readonly ICsvParser<ObserverImportModel> _parser;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(IRepository<ObserverAggregate> repository,
        IRepository<ImportValidationErrors> errorRepo,
        ICsvParser<ObserverImportModel> parser,
        ILogger<Endpoint> logger)
    {
        _repository = repository;
        _errorRepo = errorRepo;
        _parser = parser;
        _logger = logger;
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
        if (parsingResult is ParsingResult2<ObserverImportModel>.Fail failedResult)
        {
            StringBuilder errors = new StringBuilder("");
            foreach (var validationFailure in failedResult.ValidationErrors.SelectMany(x => x.Errors))
            {
                errors.AppendLine(validationFailure.ErrorMessage);
            }
            string csv = CsvRowParsedHelpers<ObserverImportModel>.ConstructErrorFileContent(failedResult.Items);
            var errorSaved = await _errorRepo.AddAsync(new(ImportType.Observer, req.File.Name, csv, DateTime.Now), ct);
            return TypedResults.BadRequest(new ImportValidationErrorModel { Id = errorSaved.Id, Message = errors.ToString() });

        }

        var importedRows = parsingResult as ParsingResult2<ObserverImportModel>.Success;
        //if (importedRows == null) 
        //{
        //    ThrowError("No rows imported");
        //}
        List<ObserverAggregate> observers = importedRows!
            .Items
            .Select(x => new ObserverAggregate(x.Name, x.Email, x.Password, x.PhoneNumber))
            .ToList();
        List<ObserverAggregate> observersToAdd = new();
        foreach (var obs in observers)
        {
            var specification = new GetObserverByLoginSpecification(obs.Login);
            var hasObserverWithSameLogin = await _repository.AnyAsync(specification, ct);

            if (hasObserverWithSameLogin)
            {
                _logger.LogWarning("An Observer with email {obs.Login} already exists!", obs.Login);
            }
            else
            {
                observersToAdd.Add(obs);
            }

        }

        if (observersToAdd.Count > 0) await _repository.AddRangeAsync(observersToAdd, ct);


        return TypedResults.NoContent();
    }
}
