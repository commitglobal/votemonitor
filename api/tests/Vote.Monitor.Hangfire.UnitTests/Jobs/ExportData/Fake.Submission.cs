using System.Collections.Generic;
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

    private static SubmissionModel GenerateSubmission(Guid formId, BaseAnswer[]? answers = null, NoteModel[]? notes = null, SubmissionAttachmentModel[]? attachments = null)
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

    public static SubmissionModel Submission(Guid formId, BaseAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)
    {
        return GenerateSubmission(formId, [answer], notes, attachments);
    }

    public static SubmissionModel Submission(Guid formId)
    {
        return GenerateSubmission(formId);
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

        return GenerateSubmission(formId, answers, notes, attachments);
    }

    public static SubmissionModel PartialSubmission(Guid formId,
        (TextAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? textAnswer = null,
        (DateAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? dateAnswer = null,
        (NumberAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? numberAnswer = null,
        (RatingAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? ratingAnswer = null,
        (SingleSelectAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? singleSelectAnswer = null,
        (MultiSelectAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? multiSelectAnswer = null)
    {
        List<BaseAnswer> answers = [];
        List<NoteModel> notes = [];
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
