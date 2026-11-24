using System.Text.RegularExpressions;

namespace Vote.Monitor.Api.IntegrationTests;

/// <summary>The record's own GET, which carries the caller NGO's review stage, reviewers, severity and labels.</summary>
public static class RecordUris
{
    private static readonly Regex PerRecord =
        new(@"^/api/election-rounds/(?<er>[^/]+)/(?:severities|labels|reviews)/(?<type>\w+)/(?<id>[^/:]+)$");

    public static string Of(Guid electionRoundId, string recordType, Guid recordId) =>
        Of(electionRoundId.ToString(), recordType, recordId.ToString());

    /// <summary>
    /// The record's own GET for a per-record URI (<c>…/severities/{recordType}/{recordId}</c>,
    /// <c>…/labels/{recordType}/{recordId}</c>, <c>…/reviews/{recordType}/{recordId}</c>).
    /// </summary>
    public static string OfPerRecordUri(string uri)
    {
        var match = PerRecord.Match(uri);
        if (!match.Success)
        {
            throw new ArgumentException($"Not a per-record URI: {uri}", nameof(uri));
        }

        return Of(match.Groups["er"].Value, match.Groups["type"].Value, match.Groups["id"].Value);
    }

    private static string Of(string electionRoundId, string recordType, string recordId) => recordType switch
    {
        "FormSubmission" => $"/api/election-rounds/{electionRoundId}/form-submissions/{recordId}:v2",
        "QuickReport" => $"/api/election-rounds/{electionRoundId}/quick-reports/{recordId}",
        "IncidentReport" => $"/api/election-rounds/{electionRoundId}/incident-reports/{recordId}",
        "CitizenReport" => $"/api/election-rounds/{electionRoundId}/citizen-reports/{recordId}",
        _ => throw new ArgumentOutOfRangeException(nameof(recordType), recordType, null)
    };
}
