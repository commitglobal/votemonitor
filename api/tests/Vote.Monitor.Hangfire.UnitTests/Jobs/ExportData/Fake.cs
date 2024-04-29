using Bogus;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Jobs.ExportData.ReadModels;
using FormAggregate = Vote.Monitor.Domain.Entities.FormAggregate.Form;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public sealed class Fake
{
    public static SubmissionModel Submission(Guid formId, BaseAnswer[] answers, NoteModel[] notes, AttachmentModel[] attachments)
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
            .RuleFor(x => x.Attachments, attachments);

        return fakeSubmission.Generate();
    }

    public static SubmissionModel Submission(Guid formId, BaseAnswer answer, NoteModel[] notes, AttachmentModel[] attachments)
    {
        return Submission(formId, [answer], notes, attachments);
    }

    public static SubmissionModel Submission(Guid formId,
        TextAnswer textAnswer, NoteModel[] textAnswerNotes, AttachmentModel[] textAnswerAttachments,
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

        return Submission(formId, answers, notes, attachments);
    }


    public static FormAggregate Form(string defaultLanguage, params BaseQuestion[] questions)
    {
        return FormAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Opening, "F1", new TranslatedString(), new TranslatedString(), defaultLanguage, [], questions);
    }
}
