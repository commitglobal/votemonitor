using System.Text.Json.Serialization;

namespace Feature.Statistics.GetElectionsOverview;

public class Response
{
    [JsonPropertyName("observers")]
    public int Observers { get; set; }

    [JsonPropertyName("polling_stations")]
    public int PollingStations { get; set; }

    [JsonPropertyName("visited_polling_stations")]
    public int VisitedPollingStations { get; set; }

    [JsonPropertyName("started_forms")]
    public int StartedForms { get; set; }

    [JsonPropertyName("questions_answered")]
    public int QuestionsAnswered { get; set; }

    [JsonPropertyName("flagged_answers")]
    public int FlaggedAnswers { get; set; }

    [JsonPropertyName("minutes_monitoring")]
    public int MinutesMonitoring { get; set; }

    [JsonPropertyName("ngos")]
    public int Ngos { get; set; }

    [JsonPropertyName("level1_visited")]
    public int Level1Visited { get; set; }

    [JsonPropertyName("level2_visited")]
    public int Level2Visited { get; set; }

    [JsonPropertyName("level3_visited")]
    public int Level3Visited { get; set; }

    [JsonPropertyName("level4_visited")]
    public int Level4Visited { get; set; }

    [JsonPropertyName("level5_visited")]
    public int Level5Visited { get; set; }
}
