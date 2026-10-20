using System.Text.Json;
using Dapper;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Queries;

namespace Feature.QuickReports;

/// <summary>
/// Aggregates the quick reports matching a filter: totals over all of them, plus one entry per distinct Level1..Level5 path
/// of their polling stations, for the levels from <c>fromLevel</c> to <c>toLevel</c>. Locations are keyed by their full
/// path because names can repeat under different parents; quick reports without a value at a level (such as the ones not
/// related to a polling station) are left out of that level.
/// </summary>
public static class QuickReportsAggregateQuery
{
    public static async Task<T> ExecuteAsync<T>(
        INpgsqlConnectionFactory dbConnectionFactory,
        Guid electionRoundId,
        Guid ngoId,
        DataSource dataSource,
        FilterRule? filterRule,
        LocationLevel fromLevel,
        LocationLevel toLevel,
        CancellationToken ct) where T : QuickReportsAggregate
    {
        var filter = new FilterSqlCompiler(V2ReportFilterFields.QuickReports).Build(filterRule);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", electionRoundId);
        parameters.Add("ngoId", ngoId);
        parameters.Add("dataSource", dataSource.ToString());
        parameters.Add("fromLevel", fromLevel.Depth);
        parameters.Add("toLevel", toLevel.Depth);
        parameters.Add("incidentCategories", IncidentCategory.List.Select(x => x.Value).ToArray());
        parameters.Add("locationTypes", QuickReportLocationType.List.Select(x => x.Value).ToArray());
        parameters.Add("followUpStatuses", QuickReportFollowUpStatus.List.Select(x => x.Value).ToArray());

        filter.AddTo(parameters);

        // the totals (level 0), then one entry per location; built as a single json document
        var sql = $$"""
                    WITH
                        QUICK_REPORTS AS MATERIALIZED (
                            SELECT s."MonitoringObserverId",
                                   s."PollingStationId",
                                   s."IncidentCategory",
                                   s."LocationType",
                                   s."FollowUpStatus",
                                   NULLIF(s."Level1", '') AS l1,
                                   NULLIF(s."Level2", '') AS l2,
                                   NULLIF(s."Level3", '') AS l3,
                                   NULLIF(s."Level4", '') AS l4,
                                   NULLIF(s."Level5", '') AS l5
                            FROM "GetQuickReportEntries"(@electionRoundId, @ngoId, @dataSource) s
                            WHERE {{filter.Sql}}
                        ),
                        GROUPS AS (
                            SELECT 0 AS level, NULL::TEXT AS l1, NULL::TEXT AS l2, NULL::TEXT AS l3, NULL::TEXT AS l4, NULL::TEXT AS l5,
                                   COUNT(*) AS quick_reports, COUNT(DISTINCT "PollingStationId") AS polling_stations,
                                   COUNT(DISTINCT "MonitoringObserverId") AS observers,
                                   ARRAY_AGG("IncidentCategory") AS incident_categories, ARRAY_AGG("LocationType") AS location_types,
                                   ARRAY_AGG("FollowUpStatus") AS follow_up_statuses
                            FROM QUICK_REPORTS
                            UNION ALL
                            SELECT 1, l1, NULL, NULL, NULL, NULL,
                                   COUNT(*), COUNT(DISTINCT "PollingStationId"), COUNT(DISTINCT "MonitoringObserverId"),
                                   ARRAY_AGG("IncidentCategory"), ARRAY_AGG("LocationType"), ARRAY_AGG("FollowUpStatus")
                            FROM QUICK_REPORTS
                            WHERE 1 BETWEEN @fromLevel AND @toLevel AND l1 IS NOT NULL
                            GROUP BY l1
                            UNION ALL
                            SELECT 2, l1, l2, NULL, NULL, NULL,
                                   COUNT(*), COUNT(DISTINCT "PollingStationId"), COUNT(DISTINCT "MonitoringObserverId"),
                                   ARRAY_AGG("IncidentCategory"), ARRAY_AGG("LocationType"), ARRAY_AGG("FollowUpStatus")
                            FROM QUICK_REPORTS
                            WHERE 2 BETWEEN @fromLevel AND @toLevel AND l2 IS NOT NULL
                            GROUP BY l1, l2
                            UNION ALL
                            SELECT 3, l1, l2, l3, NULL, NULL,
                                   COUNT(*), COUNT(DISTINCT "PollingStationId"), COUNT(DISTINCT "MonitoringObserverId"),
                                   ARRAY_AGG("IncidentCategory"), ARRAY_AGG("LocationType"), ARRAY_AGG("FollowUpStatus")
                            FROM QUICK_REPORTS
                            WHERE 3 BETWEEN @fromLevel AND @toLevel AND l3 IS NOT NULL
                            GROUP BY l1, l2, l3
                            UNION ALL
                            SELECT 4, l1, l2, l3, l4, NULL,
                                   COUNT(*), COUNT(DISTINCT "PollingStationId"), COUNT(DISTINCT "MonitoringObserverId"),
                                   ARRAY_AGG("IncidentCategory"), ARRAY_AGG("LocationType"), ARRAY_AGG("FollowUpStatus")
                            FROM QUICK_REPORTS
                            WHERE 4 BETWEEN @fromLevel AND @toLevel AND l4 IS NOT NULL
                            GROUP BY l1, l2, l3, l4
                            UNION ALL
                            SELECT 5, l1, l2, l3, l4, l5,
                                   COUNT(*), COUNT(DISTINCT "PollingStationId"), COUNT(DISTINCT "MonitoringObserverId"),
                                   ARRAY_AGG("IncidentCategory"), ARRAY_AGG("LocationType"), ARRAY_AGG("FollowUpStatus")
                            FROM QUICK_REPORTS
                            WHERE 5 BETWEEN @fromLevel AND @toLevel AND l5 IS NOT NULL
                            GROUP BY l1, l2, l3, l4, l5
                        ),
                        STATS AS (
                            SELECT G.level, G.l1, G.l2, G.l3, G.l4, G.l5,
                                   ROW_NUMBER() OVER (PARTITION BY G.level = 0 ORDER BY G.level, G.l1, G.l2, G.l3, G.l4, G.l5) AS id,
                                   JSONB_BUILD_OBJECT(
                                           'QuickReportCount', G.quick_reports,
                                           'PollingStationsCount', G.polling_stations,
                                           'ObserversCount', G.observers,
                                           'IncidentCategoryHistogram', (SELECT COALESCE(JSONB_OBJECT_AGG(V, (SELECT COUNT(*) FROM UNNEST(G.incident_categories) X WHERE X = V)), '{}'::JSONB)
                                                                         FROM UNNEST(@incidentCategories::TEXT[]) V),
                                           'LocationTypeHistogram', (SELECT COALESCE(JSONB_OBJECT_AGG(V, (SELECT COUNT(*) FROM UNNEST(G.location_types) X WHERE X = V)), '{}'::JSONB)
                                                                     FROM UNNEST(@locationTypes::TEXT[]) V),
                                           'FollowUpStatusHistogram', (SELECT COALESCE(JSONB_OBJECT_AGG(V, (SELECT COUNT(*) FROM UNNEST(G.follow_up_statuses) X WHERE X = V)), '{}'::JSONB)
                                                                       FROM UNNEST(@followUpStatuses::TEXT[]) V)
                                   ) AS stats
                            FROM GROUPS G
                        ),
                        -- the parent of a location is the location one level up with the same path (when that level is included)
                        LOCATIONS AS (
                            SELECT L.*, P.id AS parent_id
                            FROM STATS L
                                     LEFT JOIN STATS P ON P.level = L.level - 1
                                AND P.level > 0
                                AND P.l1 IS NOT DISTINCT FROM L.l1
                                AND (P.level < 2 OR P.l2 IS NOT DISTINCT FROM L.l2)
                                AND (P.level < 3 OR P.l3 IS NOT DISTINCT FROM L.l3)
                                AND (P.level < 4 OR P.l4 IS NOT DISTINCT FROM L.l4)
                            WHERE L.level > 0
                        )
                    SELECT ((SELECT stats FROM STATS WHERE level = 0)
                        || JSONB_BUILD_OBJECT(
                                'Locations', (SELECT COALESCE(JSONB_AGG(JSONB_BUILD_OBJECT(
                                                                                'Id', id, 'Level', 'Level' || level, 'ParentId', parent_id,
                                                                                'Level1', l1, 'Level2', l2, 'Level3', l3, 'Level4', l4, 'Level5', l5)
                                                                        || stats ORDER BY id), '[]'::JSONB)
                                              FROM LOCATIONS)))::TEXT
                    """;

        string json;
        using (var dbConnection = await dbConnectionFactory.GetOpenConnectionAsync(ct))
        {
            json = await dbConnection.QuerySingleAsync<string>(new CommandDefinition(sql, parameters, cancellationToken: ct));
        }

        return JsonSerializer.Deserialize<T>(json)!;
    }
}
