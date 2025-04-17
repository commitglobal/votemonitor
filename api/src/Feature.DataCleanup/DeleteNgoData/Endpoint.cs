using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Feature.DataCleanup.DeleteNgoData;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/ngo/{ngoId}/data:cleanup");
        DontAutoTag();
        Options(x => x.WithTags("data-cleanup"));
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await context
            .Notes
            .Where(n =>
                n.ElectionRoundId == req.ElectionRoundId
                && n.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                && n.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .ExecuteDeleteAsync(cancellationToken: ct);

        await context
            .Attachments
            .Where(a =>
                a.ElectionRoundId == req.ElectionRoundId
                && a.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                && a.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .ExecuteUpdateAsync(fs => fs.SetProperty(p => p.IsDeleted, true), cancellationToken: ct);

        await context
            .QuickReportAttachments
            .Where(qra =>
                qra.ElectionRoundId == req.ElectionRoundId
                && qra.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                && qra.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .ExecuteUpdateAsync(fs => fs.SetProperty(p => p.IsDeleted, true), cancellationToken: ct);

        await context.Database
            .ExecuteSqlInterpolatedAsync($"""
                                          UPDATE "FormSubmissions" AS fs
                                          SET "NumberOfQuestionsAnswered" = 0,
                                          "NumberOfFlaggedAnswers" = 0,
                                          "Answers" = '[]'
                                          FROM "MonitoringObservers" AS m
                                          INNER JOIN "MonitoringNgos" AS m0 ON m."MonitoringNgoId" = m0."Id"
                                          WHERE fs."MonitoringObserverId" = m."Id" 
                                            AND fs."ElectionRoundId" = {req.ElectionRoundId} 
                                            AND m0."NgoId" = {req.NgoId} 
                                            AND m0."ElectionRoundId" = {req.ElectionRoundId}
                                          """, ct);
        
        await context.Database
            .ExecuteSqlInterpolatedAsync($"""
                                          UPDATE "PollingStationInformation" AS psi
                                          SET "NumberOfQuestionsAnswered" = 0,
                                          "NumberOfFlaggedAnswers" = 0,
                                          "Answers" = '[]'
                                          FROM "MonitoringObservers" AS m
                                          INNER JOIN "MonitoringNgos" AS m0 ON m."MonitoringNgoId" = m0."Id"
                                          WHERE psi."MonitoringObserverId" = m."Id" 
                                            AND psi."ElectionRoundId" = {req.ElectionRoundId} 
                                            AND m0."NgoId" = {req.NgoId} 
                                            AND m0."ElectionRoundId" = {req.ElectionRoundId}
                                          """, ct);

        await context
            .QuickReports
            .Where(qr =>
                qr.ElectionRoundId == req.ElectionRoundId
                && qr.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                && qr.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId)
            .ExecuteDeleteAsync(cancellationToken: ct);
    }
}
