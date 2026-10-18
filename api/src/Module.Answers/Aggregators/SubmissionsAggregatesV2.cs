using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Module.Answers.Aggregators;

/// <summary>
/// A text answer or free text of an aggregate, referenced by id from <see cref="TextAnswerAggregate.AnswerIds"/>,
/// <see cref="SingleSelectAnswerAggregate.FreeTextIds"/> and <see cref="MultiSelectAnswerAggregate.FreeTextIds"/>.
/// </summary>
public record AggregateText(int Id, Guid SubmissionId, Guid ResponderId, string Value);

/// <summary>
/// A note of an aggregate, referenced by id from <see cref="BaseAnswerAggregate.NoteIds"/>.
/// </summary>
public record AggregateNote : NoteModel
{
    public int Id { get; init; }
}

/// <summary>
/// An attachment of an aggregate, referenced by id from <see cref="BaseAnswerAggregate.AttachmentIds"/>.
/// </summary>
public record AggregateAttachment : AggregatedSubmissionsAttachmentModel
{
    public int Id { get; init; }
}

/// <summary>
/// Form submissions / incident reports aggregate computed by "GetFormSubmissionsAggregate" / "GetIncidentReportsAggregate".
/// The question aggregates reference <see cref="Texts"/>, <see cref="Notes"/>, <see cref="Attachments"/> and
/// <see cref="Locations"/> by id.
/// </summary>
public class FormSubmissionsAggregateV2
{
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = null!;

    public TranslatedString Name { get; init; } = null!;
    public TranslatedString Description { get; init; } = null!;
    public string DefaultLanguage { get; init; } = null!;
    public string[] Languages { get; init; } = [];
    public IReadOnlyList<Responder> Responders { get; init; } = [];
    public int SubmissionCount { get; init; }
    public int TotalNumberOfQuestionsAnswered { get; init; }
    public int TotalNumberOfFlaggedAnswers { get; init; }
    public int PollingStationsCount { get; init; }

    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; init; } = null!;

    public AggregateText[] Texts { get; init; } = [];
    public AggregateNote[] Notes { get; init; } = [];
    public AggregateAttachment[] Attachments { get; set; } = [];

    /// <summary>
    /// Level1..Level5 paths of the submissions, referenced by <see cref="BaseAnswerAggregate.LocationAggregates"/>
    /// </summary>
    public AggregateLocation[] Locations { get; init; } = [];
}

/// <summary>
/// Citizen reports aggregate computed by "GetCitizenReportsAggregate".
/// The question aggregates reference <see cref="Texts"/>, <see cref="Notes"/>, <see cref="Attachments"/> and
/// <see cref="Locations"/> by id.
/// </summary>
public class CitizenReportFormSubmissionsAggregateV2
{
    public Guid ElectionRoundId { get; init; }
    public Guid MonitoringNgoId { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = null!;
    public FormType FormType { get; init; } = null!;
    public TranslatedString Name { get; init; } = null!;
    public TranslatedString Description { get; init; } = null!;
    public string DefaultLanguage { get; init; } = null!;
    public string[] Languages { get; init; } = [];
    public int SubmissionCount { get; init; }
    public int TotalNumberOfQuestionsAnswered { get; init; }
    public int TotalNumberOfFlaggedAnswers { get; init; }

    /// <summary>
    /// Number of distinct citizen report locations of the submissions.
    /// </summary>
    public int LocationsCount { get; init; }

    /// <summary>
    /// Aggregated answers per question id
    /// </summary>
    public IReadOnlyDictionary<Guid, BaseAnswerAggregate> Aggregates { get; init; } = null!;

    public AggregateText[] Texts { get; init; } = [];
    public AggregateNote[] Notes { get; init; } = [];
    public AggregateAttachment[] Attachments { get; set; } = [];

    /// <summary>
    /// Level1..Level5 paths of the submissions, referenced by <see cref="BaseAnswerAggregate.LocationAggregates"/>
    /// </summary>
    public AggregateLocation[] Locations { get; init; } = [];
}
