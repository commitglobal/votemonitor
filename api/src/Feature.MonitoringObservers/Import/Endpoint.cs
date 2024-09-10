using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Parser;
using Feature.MonitoringObservers.Services;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.Parser;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;

namespace Feature.MonitoringObservers.Import;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IObserverImportService importService,
    ICsvParser<MonitoringObserverImportModel> parser)
    : Endpoint<Request>
{
    private const string? ParsingFailedErrorMessage =
        "The file contains errors! Please use the ID to get the file with the errors described inside.";

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-observers:import");
        DontAutoTag();
        Options(x => x
            .WithTags("monitoring-observers")
            .Produces(204)
            .Produces<ImportValidationErrorModel>(400));
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var parsingResult = parser.Parse(req.File.OpenReadStream());

        switch (parsingResult)
        {
            case ParsingResult<MonitoringObserverImportModel>.Fail failedResult:
                await HandleParsingFailedAsync(req.File.FileName, failedResult, ct);
                return;

            case ParsingResult<MonitoringObserverImportModel>.Success(var observers):
                await importService.ImportAsync(req.ElectionRoundId, req.NgoId, observers, ct);
                await SendNoContentAsync(ct);
                return;
        }
    }


    private async Task HandleParsingFailedAsync(string fileName,
        ParsingResult<MonitoringObserverImportModel>.Fail failedResult, CancellationToken ct)
    {
        string csv = failedResult.Items.ConstructErrorFileContent();
        var importValidationErrors = new ImportValidationErrors(ImportType.MonitoringObserver, fileName, csv);
        var errorSaved = await context.ImportValidationErrors.AddAsync(importValidationErrors, ct);
        await context.SaveChangesAsync(ct);

        var errorResponse = new ImportValidationErrorModel
        {
            Id = errorSaved.Entity.Id,
            Message = ParsingFailedErrorMessage
        };

        await SendAsync(errorResponse, statusCode: 400, cancellation: ct);
    }
}