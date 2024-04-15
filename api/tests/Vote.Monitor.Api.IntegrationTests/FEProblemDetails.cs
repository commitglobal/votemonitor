using System.Text.Json.Serialization;

namespace Vote.Monitor.Api.IntegrationTests;

/// <summary>
/// A helper class to deserialize FastEndpoints.ProblemDetails
/// </summary>
public sealed class FEProblemDetails
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get;  set; }
    public string Instance { get;  set; } = string.Empty;
    public string TraceId { get;  set; } = string.Empty;
    public IEnumerable<Error> Errors { get; set; } = Array.Empty<Error>();

    public sealed class Error
    {
        /// <summary>
        /// the name of the error or property of the dto that caused the error
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// the reason for the error
        /// </summary>
        public string Reason { get; init; } = string.Empty;

        /// <summary>
        /// the code of the error
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; init; }

        /// <summary>
        /// the severity of the error
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Severity { get; init; }

    }
}
