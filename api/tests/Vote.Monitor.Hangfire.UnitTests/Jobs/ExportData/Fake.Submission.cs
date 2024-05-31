using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public sealed partial class Fake
{
    private static readonly SubmissionFollowUpStatus[] _submissionFollowUpStatuses =
    [
        SubmissionFollowUpStatus.NeedsFollowUp,
        SubmissionFollowUpStatus.NotApplicable,
        SubmissionFollowUpStatus.Resolved
    ];

    private static readonly QuickReportFollowUpStatus[] _quickReportsFollowUpStatuses =
    [
        QuickReportFollowUpStatus.NeedsFollowUp,
        QuickReportFollowUpStatus.NotApplicable,
        QuickReportFollowUpStatus.Resolved
    ];

    private static SubmissionModel Submission(Guid formId, BaseAnswer[] answers, NoteModel[] notes, SubmissionAttachmentModel[] attachments)
    {
        var fakeSubmission = new Faker<SubmissionModel>()
            .RuleFor(x => x.FormId, formId)
            .RuleFor(x => x.SubmissionId, f => f.Random.Guid())
            .RuleFor(x => x.TimeSubmitted, f => f.Date.Recent(1, DateTime.UtcNow))
            .RuleFor(x => x.Level1, f => f.Lorem.Word())
            .RuleFor(x => x.Level2, f => f.Lorem.Word())
            .RuleFor(x => x.Level3, f => f.Lorem.Word())
            .RuleFor(x => x.Level4, f => f.Lorem.Word())
            .RuleFor(x => x.Level5, f => f.Lorem.Word())
            .RuleFor(x => x.Number, f => f.Lorem.Word())
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.MonitoringObserverId, f => f.Random.Guid())
            .RuleFor(x => x.Answers, answers)
            .RuleFor(x => x.Notes, notes)
            .RuleFor(x => x.FollowUpStatus, f => f.PickRandom(_submissionFollowUpStatuses))
            .RuleFor(x => x.Attachments, attachments);

        return fakeSubmission.Generate();
    }

    public static SubmissionModel Submission(Guid formId, BaseAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)
    {
        return Submission(formId, [answer], notes, attachments);
    }

    public static SubmissionModel Submission(Guid formId,
        TextAnswer textAnswer, NoteModel[] textAnswerNotes, SubmissionAttachmentModel[] textAnswerAttachments,
        DateAnswer dateAnswer, NoteModel[] dateAnswerNotes, SubmissionAttachmentModel[] dateAnswerAttachments,
        NumberAnswer numberAnswer, NoteModel[] numberAnswerNotes, SubmissionAttachmentModel[] numberAnswerAttachments,
        RatingAnswer ratingAnswer, NoteModel[] ratingAnswerNotes, SubmissionAttachmentModel[] ratingAnswerAttachments,
        SingleSelectAnswer singleSelectAnswer, NoteModel[] singleSelectAnswerNotes, SubmissionAttachmentModel[] singleSelectAnswerAttachments,
        MultiSelectAnswer multiSelectAnswer, NoteModel[] multiSelectAnswerNotes, SubmissionAttachmentModel[] multiSelectAnswerAttachments)
    {
        BaseAnswer[] answers = [
            textAnswer,
            dateAnswer,
            numberAnswer,
            ratingAnswer,
            singleSelectAnswer,
            multiSelectAnswer
        ];

        NoteModel[] notes = [
            .. textAnswerNotes,
            .. dateAnswerNotes,
            .. numberAnswerNotes,
            .. ratingAnswerNotes,
            .. singleSelectAnswerNotes,
            .. multiSelectAnswerNotes
        ];

        SubmissionAttachmentModel[] attachments = [
            .. textAnswerAttachments,
            .. dateAnswerAttachments,
            .. numberAnswerAttachments,
            .. ratingAnswerAttachments,
            .. singleSelectAnswerAttachments,
            .. multiSelectAnswerAttachments
        ];

        return Submission(formId, answers, notes, attachments);
    }

}
