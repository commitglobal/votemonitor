using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    private static readonly SubmissionFollowUpStatus[] _submissionFollowUpStatuses =
    [
        SubmissionFollowUpStatus.NeedsFollowUp,
        SubmissionFollowUpStatus.NotApplicable,
        SubmissionFollowUpStatus.Resolved
    ];

    private static SubmissionModel GenerateSubmission(Guid formId, BaseAnswer[]? answers = null,
        SubmissionNoteModel[]? notes = null, SubmissionAttachmentModel[]? attachments = null)
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
            .RuleFor(x => x.Answers, answers ?? [])
            .RuleFor(x => x.Notes, notes ?? [])
            .RuleFor(x => x.FollowUpStatus, f => f.PickRandom(_submissionFollowUpStatuses))
            .RuleFor(x => x.Attachments, attachments ?? []);

        return fakeSubmission.Generate();
    }

    public static SubmissionModel Submission(Guid formId, BaseAnswer answer, SubmissionNoteModel[] notes,
        SubmissionAttachmentModel[] attachments)
    {
        return GenerateSubmission(formId, [answer], notes, attachments);
    }

    public static SubmissionModel Submission(Guid formId)
    {
        return GenerateSubmission(formId);
    }

    public static SubmissionModel Submission(Guid formId,
        TextAnswer textAnswer, SubmissionNoteModel[] textAnswerNotes, SubmissionAttachmentModel[] textAnswerAttachments,
        DateAnswer dateAnswer, SubmissionNoteModel[] dateAnswerNotes, SubmissionAttachmentModel[] dateAnswerAttachments,
        NumberAnswer numberAnswer, SubmissionNoteModel[] numberAnswerNotes,
        SubmissionAttachmentModel[] numberAnswerAttachments,
        RatingAnswer ratingAnswer, SubmissionNoteModel[] ratingAnswerNotes,
        SubmissionAttachmentModel[] ratingAnswerAttachments,
        SingleSelectAnswer singleSelectAnswer, SubmissionNoteModel[] singleSelectAnswerNotes,
        SubmissionAttachmentModel[] singleSelectAnswerAttachments,
        MultiSelectAnswer multiSelectAnswer, SubmissionNoteModel[] multiSelectAnswerNotes,
        SubmissionAttachmentModel[] multiSelectAnswerAttachments)
    {
        BaseAnswer[] answers =
        [
            textAnswer,
            dateAnswer,
            numberAnswer,
            ratingAnswer,
            singleSelectAnswer,
            multiSelectAnswer
        ];

        SubmissionNoteModel[] notes =
        [
            .. textAnswerNotes,
            .. dateAnswerNotes,
            .. numberAnswerNotes,
            .. ratingAnswerNotes,
            .. singleSelectAnswerNotes,
            .. multiSelectAnswerNotes
        ];

        SubmissionAttachmentModel[] attachments =
        [
            .. textAnswerAttachments,
            .. dateAnswerAttachments,
            .. numberAnswerAttachments,
            .. ratingAnswerAttachments,
            .. singleSelectAnswerAttachments,
            .. multiSelectAnswerAttachments
        ];

        return GenerateSubmission(formId, answers, notes, attachments);
    }

    public static SubmissionModel PartialSubmission(Guid formId,
        (TextAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? textAnswer = null,
        (DateAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? dateAnswer = null,
        (NumberAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? numberAnswer =
            null,
        (RatingAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? ratingAnswer =
            null,
        (SingleSelectAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)?
            singleSelectAnswer = null,
        (MultiSelectAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)?
            multiSelectAnswer = null)
    {
        List<BaseAnswer> answers = [];
        List<SubmissionNoteModel> notes = [];
        List<SubmissionAttachmentModel> attachments = [];

        if (textAnswer != null)
        {
            answers.Add(textAnswer.Value.answer);
            notes.AddRange(textAnswer.Value.notes);
            attachments.AddRange(textAnswer.Value.attachments);
        }

        if (dateAnswer != null)
        {
            answers.Add(dateAnswer.Value.answer);
            notes.AddRange(dateAnswer.Value.notes);
            attachments.AddRange(dateAnswer.Value.attachments);
        }

        if (numberAnswer != null)
        {
            answers.Add(numberAnswer.Value.answer);
            notes.AddRange(numberAnswer.Value.notes);
            attachments.AddRange(numberAnswer.Value.attachments);
        }

        if (ratingAnswer != null)
        {
            answers.Add(ratingAnswer.Value.answer);
            notes.AddRange(ratingAnswer.Value.notes);
            attachments.AddRange(ratingAnswer.Value.attachments);
        }

        if (singleSelectAnswer != null)
        {
            answers.Add(singleSelectAnswer.Value.answer);
            notes.AddRange(singleSelectAnswer.Value.notes);
            attachments.AddRange(singleSelectAnswer.Value.attachments);
        }

        if (multiSelectAnswer != null)
        {
            answers.Add(multiSelectAnswer.Value.answer);
            notes.AddRange(multiSelectAnswer.Value.notes);
            attachments.AddRange(multiSelectAnswer.Value.attachments);
        }

        return GenerateSubmission(formId, answers.ToArray(), notes.ToArray(), attachments.ToArray());
    }
}