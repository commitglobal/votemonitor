using System.Text.Json.Serialization;

namespace Feature.Severity.Models;

/// <summary>A record's severity in the caller's NGO.</summary>
public record SeverityModel
{
    public required SeverityLevel Severity { get; init; }

    /// <summary>When the severity was last changed; null when it was never set.</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public DateTime? ChangedAt { get; init; }
}
