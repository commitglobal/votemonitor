using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Helpers;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.Form.Submissions.ListAll;

public record FormSubmissionAdminRowModel
{
    public Guid SubmissionId { get; init; }
    public DateTime TimeSubmitted { get; init; }
    public Guid FormId { get; init; }
    public string FormCode { get; init; } = string.Empty;

    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; } = null!;

    public TranslatedString FormName { get; init; } = null!;
    public string DefaultLanguage { get; init; } = string.Empty;
    public Guid PollingStationId { get; init; }
    public string Level1 { get; init; } = string.Empty;
    public string Level2 { get; init; } = string.Empty;
    public string Level3 { get; init; } = string.Empty;
    public string Level4 { get; init; } = string.Empty;
    public string Level5 { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public Guid MonitoringNgoId { get; init; }
    public string NgoName { get; init; } = string.Empty;
    public Guid MonitoringObserverId { get; init; }
    public string ObserverName { get; init; } = string.Empty;
    public string ObserverEmail { get; init; } = string.Empty;
    public string ObserverPhoneNumber { get; init; } = string.Empty;
    public int NumberOfQuestionsAnswered { get; init; }
    public int NumberOfFlaggedAnswers { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<SubmissionFollowUpStatus, string>))]
    public SubmissionFollowUpStatus FollowUpStatus { get; init; } = null!;

    public bool IsCompleted { get; init; }

    /// <summary>
    /// Which of the observer's submissions of this form at this polling station this is, by creation (1 for PSI forms).
    /// </summary>
    public int SubmissionNumber { get; init; }

    /// <summary>How many submissions of this form the observer sent from this polling station (1 for PSI forms).</summary>
    public int SubmissionCount { get; init; }

    /// <summary>Platform admins see who sent a submission (name and email), never their phone number.</summary>
    public FormSubmissionAdminRowModel WithMaskedPhoneNumber() => this with
    {
        ObserverPhoneNumber = PersonalDataMasker.MaskPhoneNumber(ObserverPhoneNumber)
    };
}
