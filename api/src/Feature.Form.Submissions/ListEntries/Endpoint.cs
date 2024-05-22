using Authorization.Policies;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Specifications;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(INpgsqlConnectionFactory dbConnectionFactory) : Endpoint<Request, PagedResponse<FormSubmissionEntry>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x =>
        {
            x.Summary = "Lists form submissions by entry in our system";
        });
    }

    public override async Task<PagedResponse<FormSubmissionEntry>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var sql = @"
        SELECT SUM(count)
        FROM
            (SELECT count(*) AS count
             FROM ""PollingStationInformation"" psi
             INNER JOIN ""PollingStations"" ps ON ps.""Id"" = psi.""PollingStationId""
             INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             INNER JOIN ""Observers"" o ON o.""Id"" = mo.""ObserverId""
             INNER JOIN ""AspNetUsers"" u ON u.""Id"" = o.""ApplicationUserId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" = @ngoId
                 AND (@monitoringObserverId IS NULL OR mo.""Id"" = @monitoringObserverId)
                 AND (@searchText IS NULL OR @searchText = '' OR u.""FirstName"" ILIKE @searchText OR u.""LastName"" ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
                 AND (@formType IS NULL OR 'PSI' = @formType)
                 AND (@level1 IS NULL OR ps.""Level1"" = @level1)
                 AND (@level2 IS NULL OR ps.""Level2"" = @level2)
                 AND (@level3 IS NULL OR ps.""Level3"" = @level3)
                 AND (@level4 IS NULL OR ps.""Level4"" = @level4)
                 AND (@level5 IS NULL OR ps.""Level5"" = @level5)
                 AND (@pollingStationNumber IS NULL OR ps.""Number"" = @pollingStationNumber)
                 AND (@hasFlaggedAnswers is NULL OR @hasFlaggedAnswers = false OR 1 = 2)
                 AND (@followUpStatus is NULL OR 1 = 2)
             UNION ALL SELECT count(*) AS count
             FROM ""FormSubmissions"" fs
             INNER JOIN ""Forms"" f ON f.""Id"" = fs.""FormId""
             INNER JOIN ""PollingStations"" ps ON ps.""Id"" = fs.""PollingStationId""
             INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = fs.""MonitoringObserverId""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             INNER JOIN ""Observers"" o ON o.""Id"" = mo.""ObserverId""
             INNER JOIN ""AspNetUsers"" u ON u.""Id"" = o.""ApplicationUserId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" = @ngoId
                 AND (@monitoringObserverId IS NULL OR mo.""Id"" = @monitoringObserverId)
                 AND (@searchText IS NULL OR @searchText = '' OR u.""FirstName"" ILIKE @searchText OR u.""LastName"" ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
                 AND (@formType IS NULL OR f.""FormType"" = @formType)
                 AND (@level1 IS NULL OR ps.""Level1"" = @level1)
                 AND (@level2 IS NULL OR ps.""Level2"" = @level2)
                 AND (@level3 IS NULL OR ps.""Level3"" = @level3)
                 AND (@level4 IS NULL OR ps.""Level4"" = @level4)
                 AND (@level5 IS NULL OR ps.""Level5"" = @level5)
                 AND (@pollingStationNumber IS NULL OR ps.""Number"" = @pollingStationNumber)
                 AND (@hasFlaggedAnswers is NULL OR (fs.""NumberOfFlaggedAnswers"" = 0 AND @hasFlaggedAnswers = false) OR (""NumberOfFlaggedAnswers"" > 0 AND @hasFlaggedAnswers = true))
                 AND (@followUpStatus is NULL OR ""FollowUpStatus"" = @followUpStatus)
        ) c;

        WITH submissions AS
            (SELECT psi.""Id"" AS ""SubmissionId"",
                    'PSI' AS ""FormType"",
                    'PSI' AS ""FormCode"",
                    psi.""PollingStationId"",
                    psi.""MonitoringObserverId"",
                    psi.""NumberOfQuestionsAnswered"",
                    0 AS ""NumberOfFlaggedAnswers"",
                    0 AS ""MediaFilesCount"",
                    0 AS ""NotesCount"",
                    COALESCE(psi.""LastModifiedOn"", psi.""CreatedOn"") ""TimeSubmitted"",
                    'NotApplicable' AS ""FollowUpStatus""
             FROM ""PollingStationInformation"" psi
             INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = psi.""MonitoringObserverId""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" =@ngoId
                 AND (@monitoringObserverId IS NULL OR mo.""Id"" =@monitoringObserverId)
             UNION ALL SELECT fs.""Id"" AS ""SubmissionId"",
                              f.""FormType"" AS ""FormType"",
                              f.""Code"" AS ""FormCode"",
                              fs.""PollingStationId"",
                              fs.""MonitoringObserverId"",
                              fs.""NumberOfQuestionsAnswered"",
                              fs.""NumberOfFlaggedAnswers"",
                              (SELECT COUNT(1) FROM ""Attachments"" WHERE ""FormId"" = fs.""FormId"" AND ""MonitoringObserverId"" = fs.""MonitoringObserverId"" AND  fs.""PollingStationId"" = ""PollingStationId"") AS ""MediaFilesCount"",
                              (SELECT COUNT(1) FROM ""Notes"" WHERE ""FormId"" = fs.""FormId"" AND ""MonitoringObserverId"" = fs.""MonitoringObserverId"" AND  fs.""PollingStationId"" = ""PollingStationId"") AS ""NotesCount"",
                              COALESCE(fs.""LastModifiedOn"", fs.""CreatedOn"") ""TimeSubmitted"",
                              fs.""FollowUpStatus""
             FROM ""FormSubmissions"" fs
             INNER JOIN ""MonitoringObservers"" mo ON fs.""MonitoringObserverId"" = mo.""Id""
             INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
             INNER JOIN ""Forms"" f ON f.""Id"" = fs.""FormId""
             WHERE mn.""ElectionRoundId"" = @electionRoundId
                 AND mn.""NgoId"" =@ngoId
                 AND (@monitoringObserverId IS NULL OR mo.""Id"" =@monitoringObserverId))

        SELECT s.""SubmissionId"",
               s.""TimeSubmitted"",
               s.""FormCode"",
               s.""FormType"",
               ps.""Id"" AS ""PollingStationId"",
               ps.""Level1"",
               ps.""Level2"",
               ps.""Level3"",
               ps.""Level4"",
               ps.""Level5"",
               ps.""Number"",
               s.""MonitoringObserverId"",
               u.""FirstName"" || ' ' || u.""LastName"" ""ObserverName"",
               u.""Email"",
               u.""PhoneNumber"",
               mo.""Tags"",
               s.""NumberOfQuestionsAnswered"",
               s.""NumberOfFlaggedAnswers"",
               s.""MediaFilesCount"",
               s.""NotesCount"",
               s.""FollowUpStatus""
        FROM submissions s
        INNER JOIN ""PollingStations"" ps ON ps.""Id"" = s.""PollingStationId""
        INNER JOIN ""MonitoringObservers"" mo ON mo.""Id"" = s.""MonitoringObserverId""
        INNER JOIN ""MonitoringNgos"" mn ON mn.""Id"" = mo.""MonitoringNgoId""
        INNER JOIN ""Observers"" o ON o.""Id"" = mo.""ObserverId""
        INNER JOIN ""AspNetUsers"" u ON u.""Id"" = o.""ApplicationUserId""
        WHERE mn.""ElectionRoundId"" = @electionRoundId
            AND mn.""NgoId"" = @ngoId
            AND (@monitoringObserverId IS NULL OR mo.""Id"" = @monitoringObserverId)
            AND (@searchText IS NULL OR @searchText = '' OR u.""FirstName"" ILIKE @searchText OR u.""LastName"" ILIKE @searchText OR u.""Email"" ILIKE @searchText OR u.""PhoneNumber"" ILIKE @searchText)
            AND (@formType IS NULL OR s.""FormType"" = @formType)
            AND (@level1 IS NULL OR ps.""Level1"" = @level1)
            AND (@level2 IS NULL OR ps.""Level2"" = @level2)
            AND (@level3 IS NULL OR ps.""Level3"" = @level3)
            AND (@level4 IS NULL OR ps.""Level4"" = @level4)
            AND (@level5 IS NULL OR ps.""Level5"" = @level5)
            AND (@pollingStationNumber IS NULL OR ps.""Number"" = @pollingStationNumber)
            AND (@hasFlaggedAnswers is NULL OR (""NumberOfFlaggedAnswers"" = 0 AND @hasFlaggedAnswers = false) OR (""NumberOfFlaggedAnswers"" > 0 AND @hasFlaggedAnswers = true))
            AND (@followUpStatus is NULL OR ""FollowUpStatus"" = @followUpStatus)
        ORDER BY
              CASE WHEN @sortExpression = 'TimeSubmitted ASC' THEN s.""TimeSubmitted"" END ASC,
              CASE WHEN @sortExpression = 'TimeSubmitted DESC' THEN s.""TimeSubmitted"" END DESC,

              CASE WHEN @sortExpression = 'FormCode ASC' THEN s.""FormCode"" END ASC,
              CASE WHEN @sortExpression = 'FormCode DESC' THEN s.""FormCode"" END DESC,

              CASE WHEN @sortExpression = 'FormType ASC' THEN s.""FormType"" END ASC,
              CASE WHEN @sortExpression = 'FormType DESC' THEN s.""FormType"" END DESC,

              CASE WHEN @sortExpression = 'Level1 ASC' THEN ps.""Level1"" END ASC,
              CASE WHEN @sortExpression = 'Level1 DESC' THEN ps.""Level1"" END DESC,
              
              CASE WHEN @sortExpression = 'Level2 ASC' THEN ps.""Level2"" END ASC,
              CASE WHEN @sortExpression = 'Level2 DESC' THEN ps.""Level2"" END DESC,

              CASE WHEN @sortExpression = 'Level3 ASC' THEN ps.""Level3"" END ASC,
              CASE WHEN @sortExpression = 'Level3 DESC' THEN ps.""Level3"" END DESC,

              CASE WHEN @sortExpression = 'Level4 ASC' THEN ps.""Level4"" END ASC,
              CASE WHEN @sortExpression = 'Level4 DESC' THEN ps.""Level4"" END DESC,

              CASE WHEN @sortExpression = 'Level5 ASC' THEN ps.""Level5"" END ASC,
              CASE WHEN @sortExpression = 'Level5 DESC' THEN ps.""Level5"" END DESC,

              CASE WHEN @sortExpression = 'Number ASC' THEN ps.""Number"" END ASC,
              CASE WHEN @sortExpression = 'Number DESC' THEN ps.""Number"" END DESC,

              CASE WHEN @sortExpression = 'ObserverName ASC' THEN u.""FirstName"" || ' ' || u.""LastName"" END ASC,
              CASE WHEN @sortExpression = 'ObserverName DESC' THEN u.""FirstName"" || ' ' || u.""LastName"" END DESC,

              CASE WHEN @sortExpression = 'Email ASC' THEN u.""Email"" END ASC,
              CASE WHEN @sortExpression = 'Email DESC' THEN u.""Email"" END DESC,

              CASE WHEN @sortExpression = 'PhoneNumber ASC' THEN u.""PhoneNumber"" END ASC,
              CASE WHEN @sortExpression = 'PhoneNumber DESC' THEN u.""PhoneNumber"" END DESC,

              CASE WHEN @sortExpression = 'Tags ASC' THEN mo.""Tags"" END ASC,
              CASE WHEN @sortExpression = 'Tags DESC' THEN mo.""Tags"" END DESC,

              CASE WHEN @sortExpression = 'NumberOfQuestionsAnswered ASC' THEN s.""NumberOfQuestionsAnswered"" END ASC,
              CASE WHEN @sortExpression = 'NumberOfQuestionsAnswered DESC' THEN s.""NumberOfQuestionsAnswered"" END DESC,

              CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers ASC' THEN s.""NumberOfFlaggedAnswers"" END ASC,
              CASE WHEN @sortExpression = 'NumberOfFlaggedAnswers DESC' THEN s.""NumberOfFlaggedAnswers"" END DESC,

              CASE WHEN @sortExpression = 'MediaFilesCount ASC' THEN s.""MediaFilesCount"" END ASC,
              CASE WHEN @sortExpression = 'MediaFilesCount DESC' THEN s.""MediaFilesCount"" END DESC,

              CASE WHEN @sortExpression = 'NotesCount ASC' THEN s.""NotesCount"" END ASC,
              CASE WHEN @sortExpression = 'NotesCount DESC' THEN s.""NotesCount"" END DESC
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY;";

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            offset = PaginationHelper.CalculateSkip(req.PageSize, req.PageNumber),
            pageSize = req.PageSize,
            monitoringObserverId = req.MonitoringObserverId,
            searchText = $"%{req.SearchText?.Trim() ?? string.Empty}%",
            formType = req.FormTypeFilter?.ToString(),
            level1 = req.Level1Filter,
            level2 = req.Level2Filter,
            level3 = req.Level3Filter,
            level4 = req.Level4Filter,
            level5 = req.Level5Filter,
            pollingStationNumber = req.PollingStationNumberFilter,
            hasFlaggedAnswers = req.HasFlaggedAnswers,
            followUpStatus = req.FollowUpStatus?.ToString(),
            sortExpression = GetSortExpression(req.SortColumnName, req.IsAscendingSorting)
        };

        var multi = await dbConnectionFactory.GetOpenConnection().QueryMultipleAsync(sql, queryArgs);
        var totalRowCount = multi.Read<int>().Single();
        var entries = multi.Read<FormSubmissionEntry>().ToList();

        return new PagedResponse<FormSubmissionEntry>(entries, totalRowCount, req.PageNumber, req.PageSize);
    }

    private static string GetSortExpression(string? sortColumnName, bool isAscendingSorting)
    {
        if (string.IsNullOrWhiteSpace(sortColumnName))
        {
            return $"{nameof(FormSubmissionEntry.TimeSubmitted)} DESC";
        }

        var sortOrder = isAscendingSorting ? "ASC" : "DESC";

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormCode), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.FormCode)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.FormType), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.FormType)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level1), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level1)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level2), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level2)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level3), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level3)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level4), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level4)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Level5), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Level5)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Number), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Number)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.ObserverName), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.ObserverName)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Email), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Email)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.PhoneNumber), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.PhoneNumber)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.Tags), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.Tags)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfQuestionsAnswered), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NumberOfQuestionsAnswered)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NumberOfFlaggedAnswers), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NumberOfFlaggedAnswers)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.MediaFilesCount), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.MediaFilesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.NotesCount), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.NotesCount)} {sortOrder}";
        }

        if (string.Equals(sortColumnName, nameof(FormSubmissionEntry.TimeSubmitted), StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{nameof(FormSubmissionEntry.TimeSubmitted)} {sortOrder}";
        }

        return $"{nameof(FormSubmissionEntry.TimeSubmitted)} DESC";
    }
}
