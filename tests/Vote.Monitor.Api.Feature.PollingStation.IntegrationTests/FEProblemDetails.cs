using System.Text.Json.Serialization;

namespace Vote.Monitor.Api.Feature.PollingStation.IntegrationTests;

/// <summary>
/// A helper class to deserialize FastEndpoints.ProblemDetails
/// </summary>
public sealed class FEProblemDetails
{
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get;  set; }
    public string Instance { get;  set; }
    public string TraceId { get;  set; }
    public IEnumerable<Error> Errors { get;  set; }

    public sealed class Error
    {
        /// <summary>
        /// the name of the error or property of the dto that caused the error
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// the reason for the error
        /// </summary>
        public string Reason { get; init; }

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
