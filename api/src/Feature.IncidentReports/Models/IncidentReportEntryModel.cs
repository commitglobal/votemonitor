﻿using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Models;

public record IncidentReportEntryModel
{
    public Guid IncidentReportId { get; set; }
    public DateTime TimeSubmitted { get; set; }
    public string FormCode { get; set; }

    public TranslatedString FormName { get; set; }

    public string FormDefaultLanguage { get; set; }
    public string ObserverName { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
    public string[] Tags { get; set; } = [];
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfFlaggedAnswers { get; set; }
    public int NotesCount { get; set; }
    public int MediaFilesCount { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportLocationType, string>))]
    public IncidentReportLocationType LocationType { get; set; }

    public string? LocationDescription { get; set; }
    public string? Level1 { get; set; }
    public string? Level2 { get; set; }
    public string? Level3 { get; set; }
    public string? Level4 { get; set; }
    public string? Level5 { get; set; }
    public string? PollingStationNumber { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<IncidentReportFollowUpStatus, string>))]
    public IncidentReportFollowUpStatus FollowUpStatus { get; set; }

    public bool IsCompleted { get; set; }
}
