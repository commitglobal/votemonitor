using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Parser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.Parser;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.Import;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    ICsvParser<MonitoringObserverImportModel> parser,
    ILogger<Endpoint> logger)
    : Endpoint<Request>
{
    private const string? ParsingFailedErrorMessage = "The file contains errors! Please use the ID to get the file with the errors described inside.";

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/monitoring-ngos/{monitoringNgoId}/monitoring-observers:import");
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
                await HandleParsingSucceedAsync(req, observers, ct);
                return;
        }
    }

    private async Task HandleParsingSucceedAsync(Request req, List<MonitoringObserverImportModel> observers, CancellationToken ct)
    {
        var logins = observers.Select(x => x.Email).ToList();

        var monitoringNgo = await context.MonitoringNgos
            .Include(x => x.MonitoringObservers)
            .ThenInclude(x => x.Observer)
            .Where(x => x.Id == req.MonitoringNgoId)
            .FirstAsync(ct);

        var existingMonitoringObservers = monitoringNgo.MonitoringObservers.Select(x => x.Observer.ApplicationUser.Email).ToList();

        // Get observers with an account.
        var existingObservers = await context
            .Observers
            .Include(x => x.ApplicationUser)
            .Where(x => logins.Contains(x.ApplicationUser.Email))
            .Select(x => new { x.Id, x.ApplicationUser.Email })
            .ToListAsync(ct);

        var monitoringObservers = existingObservers
            .Where(o => !existingMonitoringObservers.Contains(o.Email))
            .Select(observer => MonitoringObserverAggregate.Create(req.MonitoringNgoId, observer.Id))
            .ToList();

        await context.MonitoringObservers.AddRangeAsync(monitoringObservers, ct);
        // TODO: Send notifications!

        var newObservers = observers
                .Where(o => !existingObservers.Any(x => string.Equals(x.Email, o.Email, StringComparison.InvariantCultureIgnoreCase)))
               .Select(x =>
               {

                   var user = ApplicationUser.CreateForInvite(x.FirstName, x.LastName, x.Email, x.PhoneNumber);
                   var observer = ObserverAggregate.Create(user);
                   var monitoringObserver = MonitoringObserver.Create(req.MonitoringNgoId, observer);
                   return (user, observer, monitoringObserver);
               })
               .ToList();

        foreach (var newObserver in newObservers)
        {
            var result = await userManager.CreateAsync(newObserver.user);
            if (!result.Succeeded)
            {
                logger.LogError("Errors when importing monitoring observer {email} {@errors}", newObserver.user, result.Errors);
            }

            await context.Observers.AddAsync(newObserver.observer, ct);
            await context.MonitoringObservers.AddAsync(newObserver.monitoringObserver, ct);

        }


        // TODO: send mails 

        await context.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }

    private async Task HandleParsingFailedAsync(string fileName, ParsingResult<MonitoringObserverImportModel>.Fail failedResult, CancellationToken ct)
    {
        string csv = failedResult.Items.ConstructErrorFileContent();
        var importValidationErrors = new ImportValidationErrors(ImportType.MonitoringObserver, fileName, csv);
        var errorSaved = await context.ImportValidationErrors.AddAsync(importValidationErrors, ct);

        var errorResponse = new ImportValidationErrorModel
        {
            Id = errorSaved.Entity.Id,
            Message = ParsingFailedErrorMessage
        };

        await SendAsync(errorResponse, cancellation: ct);
    }
}
