
using Dapper;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Feature.Statistics.Get;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/statistics");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets submission f OR a polling station";
        });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        string commandText = @"
        select histogram.bucket,
               coalesce(count(fs.""Id""), 0) as ""formsSubmitted"",
               coalesce(sum(fs.""NumberOfQuestionAnswered""), 0) as ""numberOfQuestionAnswered"",
               coalesce(sum(fs.""NumberOfFlaggedAnswers""), 0) as ""numberOfFlaggedAnswers"",
               coalesce(count(psn.""Id""), 0) as ""pollingStationNotesCount"",
               coalesce(count(psa.""Id""), 0) as ""pollingStationAttachmentsCount"",
               coalesce(count(psi.""Id""), 0) as ""pollingStationsInformationCount"",
               coalesce(count(DISTINCT ps.""Id""), 0) as ""visitedPollingStations"",
               coalesce(count(DISTINCT ps.""Level1""), 0) as ""distinctLevel1"",
               coalesce(count(DISTINCT ps.""Level2""), 0) as ""distinctLevel2"",
               coalesce(count(DISTINCT ps.""Level3""), 0) as ""distinctLevel3"",
               coalesce(count(DISTINCT ps.""Level4""), 0) as ""distinctLevel4"",
               coalesce(count(DISTINCT ps.""Level5""), 0) as ""distinctLevel5""
        from
            (select *
             from generate_series(date_trunc('hour', timezone('utc', now())+ interval '-12 hour'), date_trunc('hour', timezone('utc', now())), '1 hour')) as histogram(bucket)
        left join ""FormSubmissions"" fs on coalesce(fs.""LastModifiedOn"", fs.""CreatedOn"") >= histogram.bucket
        and coalesce(fs.""LastModifiedOn"", fs.""CreatedOn"") < histogram.bucket + interval '1 hour'
        left join ""PollingStationNotes"" psn on coalesce(psn.""LastModifiedOn"", psn.""CreatedOn"") >= histogram.bucket
        and coalesce(psn.""LastModifiedOn"", psn.""CreatedOn"") < histogram.bucket + interval '1 hour'
        left join ""PollingStationAttachments"" psa on coalesce(psa.""LastModifiedOn"", psa.""CreatedOn"") >= histogram.bucket
        and coalesce(psa.""LastModifiedOn"", psa.""CreatedOn"") < histogram.bucket + interval '1 hour'
        left join ""PollingStationInformation"" psi on coalesce(psi.""LastModifiedOn"", psi.""CreatedOn"") >= histogram.bucket
        and coalesce(psi.""LastModifiedOn"", psi.""CreatedOn"") < histogram.bucket + interval '1 hour'
        left join ""PollingStations"" ps on ps.""Id"" = fs.""PollingStationId""
        or ps.""Id"" = psn.""PollingStationId""
        or ps.""Id"" = psa.""PollingStationId""
        or ps.""Id"" = psi.""PollingStationId""
        left join ""MonitoringObservers"" mo on mo.""Id"" = fs.""MonitoringObserverId""
        or psa.""MonitoringObserverId"" = mo.""Id""
        or psn.""MonitoringObserverId"" = mo.""Id""
        or psi.""MonitoringObserverId"" = mo.""Id""
        left join ""MonitoringNgos"" mn on mn.""Id"" = mo.""MonitoringNgoId""
        where ps.""ElectionRoundId"" = @electionRoundId
            and mn.""NgoId"" = @monitoringNgoId
            and fs.""ElectionRoundId""= @electionRoundId
            and psn.""ElectionRoundId""= @electionRoundId
            and psi.""ElectionRoundId""= @electionRoundId
            and psa.""ElectionRoundId""= @electionRoundId
            and psa.IsDeleted = FALSE
        group by histogram.bucket
        order by histogram.bucket";

        var queryArgs = new { electionRoundId = req.ElectionRoundId , ngoId = req.NgoId};

        var histogramData = await context.Connection.QueryAsync<BucketView>(commandText, queryArgs);

        return new Response { Histogram = histogramData.ToList() };
    }
}
