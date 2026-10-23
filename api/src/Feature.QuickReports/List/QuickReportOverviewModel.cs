using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.List;

public class QuickReportOverviewModel
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportLocationType, string>))]
    public QuickReportLocationType QuickReportLocationType { get; set; }

    public DateTime Timestamp { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int NumberOfAttachments { get; set; }
    public int CommentsCount { get; set; }
    public Guid MonitoringObserverId { get; set; }
    public string ObserverName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string NgoName { get; set; }
    public string[] Tags { get; set; } = [];
    public string? PollingStationDetails { get; set; }
    public Guid? PollingStationId { get; set; }
    public string? Level1 { get; set; }
    public string? Level2 { get; set; }
    public string? Level3 { get; set; }
    public string? Level4 { get; set; }
    public string? Level5 { get; set; }
    public string? Number { get; set; }
    public string? Address { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<QuickReportFollowUpStatus, string>))]
    public QuickReportFollowUpStatus FollowUpStatus { get; set; }

    /// <summary>The record's review stage in the caller's NGO.</summary>
    public Guid StageId { get; set; }
    public string StageName { get; set; } = "";
    public string ReviewStatus { get; set; } = "";
    public DateTime? StageChangedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    /// <summary>The record's reviewers in the caller's NGO.</summary>
    public Guid[] ReviewerIds { get; set; } = [];

    /// <summary>The record's severity in the caller's NGO: None, Low, Medium, High or Urgent.</summary>
    public string Severity { get; set; } = "None";

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentCategory, string>))]
    public IncidentCategory IncidentCategory { get; set; }
}
