using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Options;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Job.Contracts.Jobs;

namespace Vote.Monitor.Hangfire.RecurringJobs;

public class PendingInviteResendJob(
    VoteMonitorContext context,
    IEmailTemplateFactory emailFactory,
    ISendEmailJob sendEmailJob,
    IOptions<ApiConfiguration> apiConfig)
{
    private readonly ApiConfiguration _apiConfig = apiConfig.Value;

    public async Task Run()
    {
        var utcToday = DateTime.UtcNow.Date;
        var day1Start = utcToday.AddDays(-1);
        var day1End = day1Start.AddDays(1);
        var day3Start = utcToday.AddDays(-3);
        var day3End = day3Start.AddDays(1);

        var pendingObservers = await context.MonitoringObservers
            .AsNoTracking()
            .Include(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Include(x => x.MonitoringNgo)
            .ThenInclude(x => x.Ngo)
            .Include(x => x.ElectionRound)
            .Where(x => x.Status == MonitoringObserverStatus.Pending)
            .Where(x =>
                (x.CreatedOn >= day1Start && x.CreatedOn < day1End) ||
                (x.CreatedOn >= day3Start && x.CreatedOn < day3End))
            .Where(x => x.Observer.ApplicationUser.InvitationToken != null)
            .Select(x => new
            {
                InvitationToken = x.Observer.ApplicationUser.InvitationToken!,
                FullName = x.Observer.ApplicationUser.DisplayName,
                Email = x.Observer.ApplicationUser.Email!,
                NgoName = x.MonitoringNgo.Ngo.Name,
                ElectionRoundDetails = x.ElectionRound.Title
            })
            .ToListAsync();

        foreach (var observer in pendingObservers)
        {
            var endpointUri = new Uri(Path.Join($"{_apiConfig.WebAppUrl}", "accept-invite"));
            var acceptInviteUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "invitationToken",
                observer.InvitationToken);

            var emailProps = new InvitationNewUserEmailProps(
                FullName: observer.FullName,
                CdnUrl: _apiConfig.WebAppUrl,
                AcceptUrl: acceptInviteUrl,
                NgoName: observer.NgoName,
                ElectionRoundDetails: observer.ElectionRoundDetails);

            var email = emailFactory.GenerateNewUserInvitationEmail(emailProps);
            await sendEmailJob.SendAsync(observer.Email, email.Subject, email.Body);
        }
    }
}
