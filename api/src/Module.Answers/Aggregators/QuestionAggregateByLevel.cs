using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Module.Answers.Aggregators;

/// <summary>
/// The answers to one question of a form, broken down by the locations of one level, computed by
/// "GetFormSubmissionsQuestionAggregateByLevel", "GetIncidentReportsQuestionAggregateByLevel" and
/// "GetCitizenReportsQuestionAggregateByLevel". The aggregates reference <see cref="Texts"/>, <see cref="Notes"/> and
/// <see cref="Attachments"/> by id.
/// </summary>
public class QuestionAggregateByLevel
{
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = null!;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = null!;

    public string DefaultLanguage { get; init; } = null!;
    public string[] Languages { get; init; } = [];

    /// <summary>
    /// The level the submissions are broken down by.
    /// </summary>
    public LocationLevel Level { get; init; } = null!;

    public int SubmissionCount { get; init; }

    /// <summary>
    /// Number of distinct polling stations of the submissions. Not set for citizen reports.
    /// </summary>
    public int? PollingStationsCount { get; init; }

    /// <summary>
    /// Number of distinct citizen report locations of the submissions. Only set for citizen reports.
    /// </summary>
    public int? LocationsCount { get; init; }

    /// <summary>
    /// The question's aggregate over all the submissions, with the question itself.
    /// </summary>
    public BaseAnswerAggregate Aggregate { get; init; } = null!;

    /// <summary>
    /// The distinct Level1..<see cref="Level"/> paths of the submissions, each with the question's aggregate for its submissions.
    /// Submissions without a value at <see cref="Level"/> are left out.
    /// </summary>
    public QuestionLocationAggregate[] Locations { get; init; } = [];

    public AggregateText[] Texts { get; init; } = [];
    public AggregateNote[] Notes { get; init; } = [];
    public AggregateAttachment[] Attachments { get; set; } = [];
}

/// <summary>
/// The question's aggregate for the submissions of one Level1..Level5 path (see <see cref="QuestionAggregateByLevel"/>).
/// </summary>
public class QuestionLocationAggregate
{
    public string? Level1 { get; init; }
    public string? Level2 { get; init; }
    public string? Level3 { get; init; }
    public string? Level4 { get; init; }
    public string? Level5 { get; init; }

    public int SubmissionCount { get; init; }
    public int TotalNumberOfQuestionsAnswered { get; init; }
    public int TotalNumberOfFlaggedAnswers { get; init; }

    /// <summary>
    /// Number of distinct polling stations of the location's submissions. Not set for citizen reports.
    /// </summary>
    public int? PollingStationsCount { get; init; }

    /// <summary>
    /// Number of distinct citizen report locations of the location's submissions. Only set for citizen reports.
    /// </summary>
    public int? LocationsCount { get; init; }

    /// <summary>
    /// Without the question itself, referencing only the texts, notes and attachments of the location's submissions.
    /// </summary>
    public BaseAnswerAggregate Aggregate { get; init; } = null!;
}
