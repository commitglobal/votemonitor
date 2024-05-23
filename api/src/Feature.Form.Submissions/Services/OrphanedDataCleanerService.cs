using Dapper;
using Vote.Monitor.Domain.ConnectionFactory;

namespace Feature.Form.Submissions.Services;

public class OrphanedDataCleanerService(INpgsqlConnectionFactory connectionFactory) : IOrphanedDataCleanerService
{
    public async Task CleanupAsync(Guid electionRoundId,
        Guid monitoringObserverId,
        Guid pollingStationId,
        Guid formId,
        Guid[] questionIds,
        CancellationToken cancellationToken = default)
    {
        using (var connection = await connectionFactory.GetOpenConnectionAsync(cancellationToken))
        {
            using (var transaction = connection.BeginTransaction())
            {
                string deleteAttachmentsSql = """
                UPDATE "Attachments"
                SET "IsDeleted" = FALSE
                WHERE "Id" IN 
                (
                    SELECT "Id" 
                    FROM "Attachments" a
                    WHERE a."ElectionRoundId" = @electionRoundId 
                    AND "MonitoringObserverId" = @monitoringObserverId
                    AND a."PollingStationId" = @pollingStationId
                    AND a."FormId"= @formId
                    AND a."QuestionId" = ANY(@questionIds)
                )
                """;

                string deleteNotesSql = """
                DELETE FROM "Notes"
                WHERE "Id" in 
                (
                    SELECT "Id" 
                    FROM "Notes" n
                    WHERE n."ElectionRoundId" = @electionRoundId 
                    AND "MonitoringObserverId" = @monitoringObserverId
                    AND n."PollingStationId" = @pollingStationId
                    AND n."FormId"= @formId
                    AND n."QuestionId" = ANY(@questionIds)
                )
                """;

                var queryParams = new
                {
                    electionRoundId,
                    monitoringObserverId,
                    pollingStationId,
                    formId,
                    questionIds 
                };

                await connection.ExecuteAsync(deleteAttachmentsSql, queryParams, transaction);
                await connection.ExecuteAsync(deleteNotesSql, queryParams, transaction);

                transaction.Commit();
            }
        }
    }
}
