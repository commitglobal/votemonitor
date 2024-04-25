using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.GetById;

public class Endpoint(VoteMonitorContext context, IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets submission by id";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        WITH submissions AS
        (SELECT psi.""Id"" as ""SubmissionId"",
        'PSI' as ""FormType"",
        'PSI' as ""FormCode"",
        psi.""PollingStationId"",
        psi.""MonitoringObserverId"",
        psi.""Answers"",
        (select ""Questions""
        from ""PollingStationInformationForms""
        where ""ElectionRoundId"" = @electionRoundId) as ""Questions"",
        '[]'::jsonb as ""Attachments"",
        '[]'::jsonb as ""Notes"",
        COALESCE(psi.""LastModifiedOn"", psi.""CreatedOn"") ""TimeSubmitted""
        FROM ""PollingStationInformation"" psi
        inner join ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
        inner join ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
        WHERE mn.""ElectionRoundId"" = @electionRoundId
            and mn.""NgoId"" = @ngoId
            and psi.""Id"" = @submissionId
        UNION ALL
        SELECT fs.""Id"" as ""SubmissionId"",
               f.""FormType"" as ""FormType"",
               f.""Code"" as ""FormCode"",
               fs.""PollingStationId"",
               fs.""MonitoringObserverId"",
               fs.""Answers"",
               f.""Questions"",

            COALESCE((select jsonb_build_array(jsonb_build_object('QuestionId', ""QuestionId"", 'FileName', ""FileName"", 'MimeType', ""MimeType"", 'FilePath', ""FilePath"", 'UploadedFileName', ""UploadedFileName"", 'TimeSubmitted', COALESCE(""LastModifiedOn"", ""CreatedOn"")))
             from ""Attachments"" a
             where a.""ElectionRoundId"" = @electionRoundId
                 and a.""FormId"" = fs.""FormId""
                 and a.""MonitoringObserverId"" = fs.""MonitoringObserverId""
                 and fs.""PollingStationId"" = a.""PollingStationId""),'[]'::JSONB) as ""Attachments"",

            COALESCE((select jsonb_build_array(jsonb_build_object('QuestionId', ""QuestionId"", 'Text', ""Text"", 'TimeSubmitted', COALESCE(""LastModifiedOn"", ""CreatedOn"")))
             from ""Notes"" n
             where n.""ElectionRoundId"" = @electionRoundId
                 and n.""FormId"" = fs.""FormId""
                 and n.""MonitoringObserverId"" = fs.""MonitoringObserverId""
                 and fs.""PollingStationId"" = n.""PollingStationId""), '[]'::JSONB) as ""Notes"",
               COALESCE(fs.""LastModifiedOn"", fs.""CreatedOn"") ""TimeSubmitted""
        FROM ""FormSubmissions"" fs
        inner join ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
        inner join ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
        inner join ""Forms"" f on f.""Id"" = fs.""FormId""
        WHERE mn.""ElectionRoundId"" = @electionRoundId
            and mn.""NgoId"" = @ngoId
            and fs.""Id"" = @submissionId
        order by ""TimeSubmitted"" desc)
        SELECT s.""SubmissionId"",
               s.""TimeSubmitted"",
               s.""FormCode"",
               s.""FormType"",
               ps.""Id"" as ""PollingStationId"",
               ps.""Level1"",
               ps.""Level2"",
               ps.""Level3"",
               ps.""Level4"",
               ps.""Level5"",
               ps.""Number"",
               s.""MonitoringObserverId"",
               u.""FirstName"",
               u.""Email"",
               u.""PhoneNumber"",
               mo.""Tags"",
               s.""Attachments"",
               s.""Notes"",
               s.""Answers"",
               s.""Questions""
        FROM submissions s
        inner join ""PollingStations"" ps on ps.""Id"" = s.""PollingStationId""
        inner join ""MonitoringObservers"" mo on mo.""Id"" = s.""MonitoringObserverId""
        inner join ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
        inner join ""Observers"" o ON o.""Id"" = mo.""ObserverId""
        inner join ""AspNetUsers"" u ON u.""Id"" = o.""ApplicationUserId""
        WHERE mn.""ElectionRoundId"" = @electionRoundId
            and mn.""NgoId"" =@ngoId
        order by ""TimeSubmitted"" desc";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            submissionId = req.SubmissionId,
        };

        var submission = await context.Connection.QueryFirstOrDefaultAsync<Response>(sql, queryArgs);
        if (submission is null)
        {
            return TypedResults.NotFound();
        }
        
        foreach (var attachment in submission.Attachments)
        {
            var result = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName, ct);
            if (result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds))
            {
                attachment.PresignedUrl = url;
                attachment.UrlValidityInSeconds = urlValidityInSeconds;
            }
        }

        return TypedResults.Ok(submission);
    }
}
