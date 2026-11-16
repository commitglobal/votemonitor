namespace Feature.Dashboards;

/// <summary>A dashboard's charts as stored: the JSON array the web app sent, unchanged.</summary>
public static class DashboardCharts
{
    /// <summary>The bytes the charts take once serialised (compact UTF-8).</summary>
    public static int Size(JsonElement charts) => JsonSerializer.SerializeToUtf8Bytes(charts).Length;

    /// <summary>The charts to store; an empty array when none were sent.</summary>
    public static JsonDocument ToDocument(JsonElement? charts) =>
        charts is { ValueKind: JsonValueKind.Array } value
            ? JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(value))
            : JsonDocument.Parse("[]");

    /// <summary>The trimmed description, or null when empty.</summary>
    public static string? Description(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
