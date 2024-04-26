using Bogus;
using NPOI.Util;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public sealed class FakeSubmission : Faker<SubmissionModel>
{
    private FakeSubmission()
    {
        RuleFor(x => x.SubmissionId, f => f.Random.Guid());
        RuleFor(x => x.TimeSubmitted, f => f.Date.Recent(1, DateTime.UtcNow));
        RuleFor(x => x.Level1, f => f.Lorem.Word());
        RuleFor(x => x.Level2, f => f.Lorem.Word());
        RuleFor(x => x.Level3, f => f.Lorem.Word());
        RuleFor(x => x.Level4, f => f.Lorem.Word());
        RuleFor(x => x.Level5, f => f.Lorem.Word());
        RuleFor(x => x.Number, f => f.Lorem.Word());
        RuleFor(x => x.FirstName, f => f.Name.FirstName());
        RuleFor(x => x.LastName, f => f.Name.LastName());
        RuleFor(x => x.Email, f => f.Internet.Email());
        RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber());
        RuleFor(x => x.MonitoringObserverId, f => f.Random.Guid());
    }

    private FakeSubmission(BaseAnswer answer, NoteModel[] notes, AttachmentModel[] attachments) : this()
    {

        RuleFor(x => x.Answers, [answer]);
        RuleFor(x => x.Notes, notes);
        RuleFor(x => x.Attachments, attachments);
    }

    private FakeSubmission(BaseAnswer[] answers, NoteModel[] notes, AttachmentModel[] attachments) : this()
    {
        RuleFor(x => x.Answers, answers);
        RuleFor(x => x.Notes, notes);
        RuleFor(x => x.Attachments, attachments);
    }

    public static SubmissionModel For(BaseAnswer answer, NoteModel[] notes, AttachmentModel[] attachments)
    {
        return new FakeSubmission(answer, notes, attachments).Generate();
    }
    public static SubmissionModel For(TextAnswer textAnswer, NoteModel[] textAnswerNotes, AttachmentModel[] textAnswerAttachments,
        DateAnswer dateAnswer, NoteModel[] dateAnswerNotes, AttachmentModel[] dateAnswerAttachments,
        NumberAnswer numberAnswer, NoteModel[] numberAnswerNotes, AttachmentModel[] numberAnswerAttachments,
        RatingAnswer ratingAnswer, NoteModel[] ratingAnswerNotes, AttachmentModel[] ratingAnswerAttachments,
        SingleSelectAnswer singleSelectAnswer, NoteModel[] singleSelectAnswerNotes, AttachmentModel[] singleSelectAnswerAttachments,
        MultiSelectAnswer multiSelectAnswer, NoteModel[] multiSelectAnswerNotes, AttachmentModel[] multiSelectAnswerAttachments)
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

        AttachmentModel[] attachments = [
            .. textAnswerAttachments,
            .. dateAnswerAttachments,
            .. numberAnswerAttachments,
            .. ratingAnswerAttachments,
            .. singleSelectAnswerAttachments,
            .. multiSelectAnswerAttachments
        ];

        return new FakeSubmission(answers, notes, attachments).Generate();
    }
}
