namespace Feature.SavedViews;

/// <summary>A view's state as stored: the JSON object the web app sent, unchanged.</summary>
public static class SavedViewState
{
    /// <summary>The bytes the state takes once serialised (compact UTF-8).</summary>
    public static int Size(JsonElement state) => JsonSerializer.SerializeToUtf8Bytes(state).Length;

    public static JsonDocument ToDocument(JsonElement state) => JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(state));

    /// <summary>The trimmed description, or null when empty.</summary>
    public static string? Description(string? description) =>
        string.IsNullOrWhiteSpace(description) ? null : description.Trim();
}
