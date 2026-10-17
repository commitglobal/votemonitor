namespace Vote.Monitor.Hangfire.Jobs.Export;

public static class CommentsSql
{
    /// <summary>
    /// Correlated subquery returning a JSONB array of comments (see <see cref="ReadModels.CommentModel"/>) for a report.
    /// Only comments authored by admins or staff of the @ngoId NGO are included (coalition members' comments are not).
    /// </summary>
    /// <param name="commentsTable">Comments table, e.g. "FormSubmissionComments".</param>
    /// <param name="reportIdColumn">Column in the comments table referencing the report, e.g. "SubmissionId".</param>
    /// <param name="reportIdExpression">SQL expression for the report id in the outer query, e.g. QR."Id".</param>
    public static string JsonAgg(string commentsTable, string reportIdColumn, string reportIdExpression) =>
        $"""
         COALESCE(
                 (
                     SELECT
                         JSONB_AGG(
                                 JSONB_BUILD_OBJECT(
                                         'AuthorName',
                                         COALESCE(CMTU."DisplayName", ''),
                                         'Text',
                                         CMT."Text",
                                         'Timestamp',
                                         COALESCE(CMT."LastModifiedOn", CMT."CreatedOn")
                                 )
                                 ORDER BY CMT."CreatedOn"
                         )
                     FROM
                         "{commentsTable}" CMT
                         LEFT JOIN "AspNetUsers" CMTU ON CMTU."Id" = CMT."CreatedBy"
                     WHERE
                         CMT."{reportIdColumn}" = {reportIdExpression}
                       AND (
                         EXISTS (SELECT 1 FROM "NgoAdmins" NA WHERE NA."ApplicationUserId" = CMT."CreatedBy" AND NA."NgoId" = @ngoId)
                         OR EXISTS (SELECT 1 FROM "NgoStaff" NS WHERE NS."ApplicationUserId" = CMT."CreatedBy" AND NS."NgoId" = @ngoId)
                       )
                 ),
                 '[]'::JSONB
         )
         """;
}
