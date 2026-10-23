using System.Text.Json;
using System.Text.Json.Serialization;

namespace Feature.Review.Models;

/// <summary>The fields every kind of automation rule has.</summary>
public abstract record AutomationRuleModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required ReportRecordType[] RecordTypes { get; init; }

    /// <summary>The advanced filter a record must match, as saved; null for every record.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public JsonDocument? Conditions { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }

    /// <summary>The admin who last saved the rule (its creator until it is edited).</summary>
    public required Guid UpdatedById { get; init; }

    public required string UpdatedByName { get; init; }
}
