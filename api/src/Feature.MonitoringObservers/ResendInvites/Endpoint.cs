using Authorization.Policies.Requirements;
using Job.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Options;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Feature.MonitoringObservers.ResendInvites;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IJobService jobService,
    IEmailTemplateFactory emailFactory,
    IOptions<ApiConfiguration> apiConfig
) : Endpoint<Request, Results<NoContent, NotFound>>
{
    private readonly ApiConfiguration _apiConfig = apiConfig.Value;

    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/monitoring-observers:resend-invites");
        Description(x => x.Accepts<Request>());
        DontAutoTag();
        Options(x => x.WithTags("monitoring-observers"));
        Summary(s =>
        {
            s.Summary = "Resends invitation mail to all pending observers or to selected ids";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var requirement = new MonitoringNgoAdminRequirement(req.ElectionRoundId);
        var authorizationResult = await authorizationService.AuthorizeAsync(User, requirement);
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var ngoName = await context
            .MonitoringNgos
            .Include(x => x.Ngo)
            .Where(x => x.NgoId == req.NgoId && x.ElectionRoundId == req.ElectionRoundId)
            .Select(x => x.Ngo.Name)
            .FirstAsync(ct);

        var electionRoundName = await context.ElectionRounds
            .Where(x => x.Id == req.ElectionRoundId)
            .Select(x => x.Title)
            .FirstAsync(ct);

        var pendingInviteMonitoringObservers = await context.MonitoringObservers
            .Include(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .Where(x => x.MonitoringNgo.NgoId == req.NgoId && x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .Where(x => x.Status == MonitoringObserverStatus.Pending)
            .Where(x => req.Ids.Count == 0 || req.Ids.Contains(x.Id))
            .Select(x => new
            {
                InvitationToken = x.Observer.ApplicationUser.InvitationToken,
                FullName = x.Observer.ApplicationUser.FirstName + " " + x.Observer.ApplicationUser.LastName,
                Email = x.Observer.ApplicationUser.Email
            }).ToListAsync(ct);

        foreach (var monitoringObserver in pendingInviteMonitoringObservers)
        {
            var endpointUri = new Uri(Path.Combine($"{_apiConfig.WebAppUrl}", "accept-invite"));

            string acceptInviteUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "invitationToken",
                monitoringObserver.InvitationToken!);

            var invitationNewUserEmailProps = new InvitationNewUserEmailProps(FullName: monitoringObserver.FullName,
                CdnUrl: _apiConfig.WebAppUrl,
                AcceptUrl: acceptInviteUrl,
                NgoName: ngoName,
                ElectionRoundDetails: electionRoundName);

            var email = emailFactory.GenerateNewUserInvitationEmail(invitationNewUserEmailProps);
            jobService.EnqueueSendEmail(monitoringObserver.Email!, email.Subject, email.Body);
        }

        return TypedResults.NoContent();
    }
}
