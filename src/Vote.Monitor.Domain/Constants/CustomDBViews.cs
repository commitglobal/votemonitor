using Vote.Monitor.Domain.Entities.PollingStationAttachmentAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

namespace Vote.Monitor.Domain.Constants;

public static class CustomDBViews
{
    /// <summary>
    /// A view to retrieve list of polling stations visited.
    /// A polling station visit is any interaction of an observer with data related to polling station
    /// <para>Example of interactions:</para>
    /// <list type="bullet">
    ///     <item>Filling in polling station information</item>
    ///     <item>Filling in any form</item>
    ///     <item>Adding a note</item>
    ///     <item>Adding an attachment</item>
    /// </list>
    /// </summary>
    public const string PollingStationVisits = "PollingStationVisits";

    public const string CreatePollingStationVisits = @$"
              CREATE OR REPLACE VIEW ""{PollingStationVisits}"" AS 
              SELECT t.""ElectionRoundId"",
              t.""PollingStationId"",
              ps.""Level1"",
              ps.""Level2"",
              ps.""Level3"",
              ps.""Level4"",
              ps.""Level5"",
              ps.""Address"",
              ps.""Number"",
              mo.""MonitoringNgoId"",
              mn.""NgoId"",
              t.""MonitoringObserverId"",
              mo.""ObserverId"",
              MIN(t.""LatestTimestamp"") ""VisitedAt""
              FROM (
                  SELECT psi.""{nameof(PollingStationInformation.ElectionRoundId)}"",
                  psi.""{nameof(PollingStationInformation.PollingStationId)}"",
                  psi.""{nameof(PollingStationInformation.MonitoringObserverId)}"",
                  COALESCE(psi.""ArrivalTime"", psi.""LastModifiedOn"", psi.""CreatedOn"") ""LatestTimestamp""
                  FROM ""{Tables.PollingStationInformation}"" psi
                  UNION
                  SELECT
                  n.""{nameof(PollingStationNote.ElectionRoundId)}"", 
                  n.""{nameof(PollingStationNote.PollingStationId)}"", 
                  n.""{nameof(PollingStationNote.MonitoringObserverId)}"", 
                  COALESCE(n.""LastModifiedOn"", n.""CreatedOn"") ""LatestTimestamp""
                  FROM ""{Tables.PollingStationNotes}"" n
                  UNION
                  SELECT
                  a.""{nameof(PollingStationAttachment.ElectionRoundId)}"", 
                  a.""{nameof(PollingStationAttachment.PollingStationId)}"", 
                  a.""{nameof(PollingStationNote.MonitoringObserverId)}"", 
                  COALESCE(a.""LastModifiedOn"", a.""CreatedOn"") ""LatestTimestamp""
                  FROM ""{Tables.PollingStationAttachments}"" a
              ) t 
              INNER JOIN ""{Tables.MonitoringObservers}"" mo ON mo.""Id"" = t.""MonitoringObserverId""
              INNER JOIN ""{Tables.MonitoringNgos}"" mn ON mo.""MonitoringNgoId"" = mn.""Id""
              INNER JOIN ""{Tables.PollingStations}"" ps ON ps.""Id"" = t.""PollingStationId""
              GROUP BY 
                    t.""ElectionRoundId"",
                    t.""PollingStationId"",
                    ps.""Level1"",
                    ps.""Level2"",
                    ps.""Level3"",
                    ps.""Level4"",
                    ps.""Level5"",
                    ps.""Address"",
                    ps.""Number"",
                    mo.""MonitoringNgoId"",
                    mn.""NgoId"", 
                    t.""MonitoringObserverId"",
                    mo.""ObserverId"";
              ";
}
