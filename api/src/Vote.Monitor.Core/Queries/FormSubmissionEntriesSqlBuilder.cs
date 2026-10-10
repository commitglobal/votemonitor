using Dapper;

namespace Vote.Monitor.Core.Queries;

public sealed class FormSubmissionEntriesFilterCriteria
{
    public Guid? CoalitionMemberId { get; init; }
    public Guid? MonitoringObserverId { get; init; }
    public string? SearchText { get; init; }
    public string? FormType { get; init; }
    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }
    public string? PollingStationNumber { get; init; }
    public Guid? PollingStationId { get; init; }
    public bool? HasFlaggedAnswers { get; init; }
    public string? FollowUpStatus { get; init; }
    public string[]? Tags { get; init; }
    public string? MonitoringObserverStatus { get; init; }
    public Guid? FormId { get; init; }
    public bool? HasNotes { get; init; }
    public bool? HasAttachments { get; init; }
    public bool? HasComments { get; init; }
    public string? QuestionsAnswered { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool? IsCompleted { get; init; }
}

public static class FormSubmissionEntriesSqlBuilder
{
    public static SqlBuilder ApplyFilters(this SqlBuilder builder, FormSubmissionEntriesFilterCriteria filters)
    {
        if (filters.MonitoringObserverId.HasValue)
        {
            builder.Where(@"s.""MonitoringObserverId"" = @monitoringObserverId",
                new { monitoringObserverId = filters.MonitoringObserverId });
        }

        if (filters.CoalitionMemberId.HasValue)
        {
            builder.Where(@"s.""NgoId"" = @coalitionMemberId",
                new { coalitionMemberId = filters.CoalitionMemberId });
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchText))
        {
            builder.Where(
                """
                (s."DisplayName" ILIKE @searchText
                 OR s."Email" ILIKE @searchText
                 OR s."PhoneNumber" ILIKE @searchText
                 OR s."MonitoringObserverId"::TEXT ILIKE @searchText)
                """,
                new { searchText = $"%{filters.SearchText.Trim()}%" });
        }

        if (!string.IsNullOrWhiteSpace(filters.FormType))
        {
            builder.Where(@"s.""FormType"" = @formType", new { formType = filters.FormType });
        }

        if (!string.IsNullOrWhiteSpace(filters.Level1))
        {
            builder.Where(@"s.""Level1"" = @level1", new { level1 = filters.Level1 });
        }

        if (!string.IsNullOrWhiteSpace(filters.Level2))
        {
            builder.Where(@"s.""Level2"" = @level2", new { level2 = filters.Level2 });
        }

        if (!string.IsNullOrWhiteSpace(filters.Level3))
        {
            builder.Where(@"s.""Level3"" = @level3", new { level3 = filters.Level3 });
        }

        if (!string.IsNullOrWhiteSpace(filters.Level4))
        {
            builder.Where(@"s.""Level4"" = @level4", new { level4 = filters.Level4 });
        }

        if (!string.IsNullOrWhiteSpace(filters.Level5))
        {
            builder.Where(@"s.""Level5"" = @level5", new { level5 = filters.Level5 });
        }

        if (!string.IsNullOrWhiteSpace(filters.PollingStationNumber))
        {
            builder.Where(@"s.""Number"" = @pollingStationNumber",
                new { pollingStationNumber = filters.PollingStationNumber });
        }

        if (filters.PollingStationId.HasValue)
        {
            builder.Where(@"s.""PollingStationId"" = @pollingStationId",
                new { pollingStationId = filters.PollingStationId });
        }

        if (filters.HasFlaggedAnswers == true)
        {
            builder.Where(@"s.""NumberOfFlaggedAnswers"" > 0");
        }
        else if (filters.HasFlaggedAnswers == false)
        {
            builder.Where(@"s.""NumberOfFlaggedAnswers"" = 0");
        }

        if (!string.IsNullOrWhiteSpace(filters.FollowUpStatus))
        {
            builder.Where(@"s.""FollowUpStatus"" = @followUpStatus",
                new { followUpStatus = filters.FollowUpStatus });
        }

        if (filters.Tags is { Length: > 0 })
        {
            builder.Where(@"s.""Tags"" && @tagsFilter", new { tagsFilter = filters.Tags });
        }

        if (!string.IsNullOrWhiteSpace(filters.MonitoringObserverStatus))
        {
            builder.Where(@"s.""MonitoringObserverStatus"" = @monitoringObserverStatus",
                new { monitoringObserverStatus = filters.MonitoringObserverStatus });
        }

        if (filters.FormId.HasValue)
        {
            builder.Where(@"s.""FormId"" = @formId", new { formId = filters.FormId });
        }

        if (filters.QuestionsAnswered == "All")
        {
            builder.Where(@"s.""NumberOfQuestions"" = s.""NumberOfQuestionsAnswered""");
        }
        else if (filters.QuestionsAnswered == "Some")
        {
            builder.Where(@"s.""NumberOfQuestions"" <> s.""NumberOfQuestionsAnswered""");
        }
        else if (filters.QuestionsAnswered == "None")
        {
            builder.Where(@"s.""NumberOfQuestionsAnswered"" = 0");
        }

        if (filters.HasNotes == true)
        {
            builder.Where(@"s.""HasNotes"" = true");
        }
        else if (filters.HasNotes == false)
        {
            builder.Where(@"s.""HasNotes"" = false");
        }

        if (filters.HasAttachments == true)
        {
            builder.Where(@"s.""HasAttachments"" = true");
        }
        else if (filters.HasAttachments == false)
        {
            builder.Where(@"s.""HasAttachments"" = false");
        }

        if (filters.HasComments == true)
        {
            builder.Where(@"s.""HasComments"" = true");
        }
        else if (filters.HasComments == false)
        {
            builder.Where(@"s.""HasComments"" = false");
        }

        if (filters.FromDate.HasValue)
        {
            builder.Where(@"s.""LastUpdatedAt"" >= @fromDate::timestamp",
                new { fromDate = filters.FromDate.Value.ToString("O") });
        }

        if (filters.ToDate.HasValue)
        {
            builder.Where(@"s.""LastUpdatedAt"" <= @toDate::timestamp",
                new { toDate = filters.ToDate.Value.ToString("O") });
        }

        if (filters.IsCompleted.HasValue)
        {
            builder.Where(@"s.""IsCompleted"" = @isCompleted", new { isCompleted = filters.IsCompleted });
        }

        return builder;
    }
}
