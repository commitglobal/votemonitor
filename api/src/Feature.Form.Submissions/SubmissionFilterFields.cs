using Vote.Monitor.Core.RulesEngine;

namespace Feature.Form.Submissions;

public static class SubmissionFilterFields
{
    public static IReadOnlyDictionary<string, FilterField> All => Vote.Monitor.Domain.Queries.V2ReportFilterFields.FormSubmissions;
}
