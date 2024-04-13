using Authorization.Policies.Requirements;
using Feature.MonitoringObservers.Parser;
using Job.Contracts;
using Job.Contracts.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
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
    ILogger<Endpoint> logger,
    IJobService jobService,
    IEmailTemplateFactory emailFactory)
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
        //var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        //var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        //if (!authorizationResult.Succeeded)
        //{
        //    await SendNotFoundAsync(ct);
        //    return;
        //}

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
        var normalizedEmails = observers.Select(x => x.Email.ToUpperInvariant()).ToList();

        var existingAccounts = await userManager
            .Users
            .Where(x => normalizedEmails.Contains(x.NormalizedEmail))
            .ToListAsync(ct);

        var existingMonitoringObservers = await context
            .MonitoringObservers
            .Include(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => normalizedEmails.Contains(x.Observer.ApplicationUser.NormalizedEmail))
            .ToListAsync(ct);

        var existingObservers = await context
            .Observers
            .Include(x => x.ApplicationUser)
            .Where(x => normalizedEmails.Contains(x.ApplicationUser.NormalizedEmail))
            .ToListAsync(ct);

        foreach (var observer in observers)
        {
            var normalizedEmail = observer.Email.ToUpperInvariant();
            // Has account in our system
            var existingAccount = existingAccounts.FirstOrDefault(x => x.NormalizedEmail == normalizedEmail);

            if (existingAccount != null)
            {
                // Check if is observer
                if (existingAccount.Role != UserRole.Observer)
                {
                    logger.LogWarning("Invited {observer} has different {role} in our system", existingAccount.Email, existingAccount.Role);
                    continue;
                }

                // Check if it is already monitoring same election but for different NGO
                var isMonitoringForAnotherNgo = existingMonitoringObservers.Any(x =>
                     x.Observer.ApplicationUser.NormalizedEmail == normalizedEmail &&
                     x.ElectionRoundId == req.ElectionRoundId);

                if (isMonitoringForAnotherNgo)
                {
                    logger.LogWarning("Invited {observer} is monitoring same {electionRound} for different ngo", existingAccount.Email, req.ElectionRoundId);
                    continue;
                }

                // Has an account is not monitoring this election round
                var existingObserver = existingObservers.FirstOrDefault(x => x.ApplicationUser.NormalizedEmail == normalizedEmail);

                if (existingObserver is null)
                {
                    // has an account but we failed to create the observer 
                    existingAccount.NewInvite();
                    var newObserver = ObserverAggregate.Create(existingAccount);
                    var newMonitoringObserver = MonitoringObserver.Create(req.ElectionRoundId, req.MonitoringNgoId, newObserver.Id, observer.Tags);
                    await context.Observers.AddAsync(newObserver, ct);
                    await context.MonitoringObservers.AddAsync(newMonitoringObserver, ct);

                    var email = emailFactory.GenerateEmail(EmailTemplateType.InvitationNewUser, new InvitationNewUserEmailProps("", existingAccount.InvitationToken!.Value.ToString()));
                    jobService.SendEmail(observer.Email, email.Subject, email.Body);
                }
                else
                {
                    var newMonitoringObserver = MonitoringObserverAggregate.Create(req.ElectionRoundId,
                        req.MonitoringNgoId, existingObserver.Id,
                        observer.Tags);

                    await context.MonitoringObservers.AddAsync(newMonitoringObserver, ct);
                    var email = emailFactory.GenerateEmail(EmailTemplateType.InvitationExistingUser,
                        new InvitationExistingUserEmailProps("", ""));
                    jobService.SendEmail(observer.Email, email.Subject, email.Body);
                }
            }
            else
            {
                // Does not have an account 
                var user = ApplicationUser.Invite(observer.FirstName, observer.LastName, observer.Email, observer.PhoneNumber);

                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    logger.LogError("Errors when importing monitoring observer {email} {@errors}", user.Email, result.Errors);
                }
                var newObserver = ObserverAggregate.Create(user);
                var newMonitoringObserver = MonitoringObserver.Create(req.ElectionRoundId, req.MonitoringNgoId, newObserver.Id, observer.Tags);
                await context.Observers.AddAsync(newObserver, ct);
                await context.MonitoringObservers.AddAsync(newMonitoringObserver, ct);

                var email = emailFactory.GenerateEmail(EmailTemplateType.InvitationNewUser, new InvitationNewUserEmailProps("", user.InvitationToken!.Value.ToString()));
                jobService.SendEmail(observer.Email, email.Subject, email.Body);
            }
        }
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
