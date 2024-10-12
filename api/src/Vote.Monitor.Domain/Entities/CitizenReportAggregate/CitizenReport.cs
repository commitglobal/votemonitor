using Vote.Monitor.Domain.Entities.CitizenReportAttachmentAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Vote.Monitor.Domain.Entities.CitizenReportAggregate;

public class CitizenReport : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public Guid FormId { get; private set; }
    public Form Form { get; private set; }
    public int NumberOfQuestionsAnswered { get; private set; }
    public int NumberOfFlaggedAnswers { get; private set; }
    public CitizenReportFollowUpStatus FollowUpStatus { get; private set; }
    public IReadOnlyList<BaseAnswer> Answers { get; private set; } = new List<BaseAnswer>().AsReadOnly();

    private readonly List<CitizenReportNote> _notes = new();
    public virtual IReadOnlyList<CitizenReportNote> Notes => _notes.ToList().AsReadOnly();

    private readonly List<CitizenReportAttachment> _attachments = new();
    public virtual IReadOnlyList<CitizenReportAttachment> Attachments => _attachments.ToList().AsReadOnly();
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; }

    private CitizenReport(
        Guid id,
        ElectionRound electionRound,
        Form form,
        Location location,
        List<BaseAnswer> answers,
        int numberOfQuestionsAnswered,
        int numberOfFlaggedAnswers) : base(id)
    {
        ElectionRound = electionRound;
        ElectionRoundId = electionRound.Id;
        Form = form;
        FormId = form.Id;
        Answers = answers.ToList().AsReadOnly();
        NumberOfQuestionsAnswered = numberOfQuestionsAnswered;
        NumberOfFlaggedAnswers = numberOfFlaggedAnswers;
        FollowUpStatus = CitizenReportFollowUpStatus.NotApplicable;
        Location = location;
        LocationId = location.Id;
    }

    internal static CitizenReport Create(
        Guid id,
        ElectionRound electionRound,
        Form form,
        Location location,
        List<BaseAnswer> answers,
        int numberOfQuestionAnswered,
        int numberOfFlaggedAnswers) =>
        new(id, electionRound, form, location, answers, numberOfQuestionAnswered, numberOfFlaggedAnswers);

    internal void Update(IEnumerable<BaseAnswer>? answers, int? numberOfQuestionsAnswered,
        int? numberOfFlaggedAnswers)
    {
        if (answers is not null)
        {
            Answers = answers.ToList().AsReadOnly();
        }

        if (numberOfFlaggedAnswers is not null)
        {
            NumberOfFlaggedAnswers = numberOfFlaggedAnswers.Value;
        }

        if (numberOfQuestionsAnswered is not null)
        {
            NumberOfQuestionsAnswered = numberOfQuestionsAnswered.Value;
        }
    }

    public void UpdateFollowUpStatus(CitizenReportFollowUpStatus followUpStatus)
    {
        FollowUpStatus = followUpStatus;
    }

    public void ClearAnswers()
    {
        Answers = [];
        NumberOfFlaggedAnswers = 0;
        NumberOfQuestionsAnswered = 0;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private CitizenReport()
    {
    }
#pragma warning restore CS8618
}