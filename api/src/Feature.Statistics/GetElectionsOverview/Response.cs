using System.Text.Json.Serialization;

namespace Feature.Statistics.GetElectionsOverview;

public class Response
{
    [JsonPropertyName("observers")]
    public long Observers { get; set; }

    [JsonPropertyName("polling_stations")]
    public long PollingStations { get; set; }

    [JsonPropertyName("visited_polling_stations")]
    public long VisitedPollingStations { get; set; }

    [JsonPropertyName("started_forms")]
    public long StartedForms { get; set; }

    [JsonPropertyName("questions_answered")]
    public long QuestionsAnswered { get; set; }

    [JsonPropertyName("flagged_answers")]
    public long FlaggedAnswers { get; set; }

    [JsonPropertyName("minutes_monitoring")]
    public long MinutesMonitoring { get; set; }

    [JsonPropertyName("ngos")]
    public long Ngos { get; set; }
}
