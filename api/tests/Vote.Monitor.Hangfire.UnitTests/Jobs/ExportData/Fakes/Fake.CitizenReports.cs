using Bogus;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Jobs.Export.CitizenReports.ReadModels;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    private static readonly CitizenReportFollowUpStatus[] _citizenReportFollowUpStatuses =
    [
        CitizenReportFollowUpStatus.NeedsFollowUp,
        CitizenReportFollowUpStatus.NotApplicable,
        CitizenReportFollowUpStatus.Resolved
    ];

    private static CitizenReportModel GenerateCitizenReport(Guid formId, BaseAnswer[]? answers = null,
        NoteModel[]? notes = null, SubmissionAttachmentModel[]? attachments = null)
    {
        var fakeCitizenReport = new Faker<CitizenReportModel>()
            .RuleFor(x => x.FormId, formId)
            .RuleFor(x => x.CitizenReportId, f => f.Random.Guid())
            .RuleFor(x => x.TimeSubmitted, f => f.Date.Recent(1, DateTime.UtcNow))
            .RuleFor(x => x.Level1, f => f.Lorem.Word())
            .RuleFor(x => x.Level2, f => f.Lorem.Word())
            .RuleFor(x => x.Level3, f => f.Lorem.Word())
            .RuleFor(x => x.Level4, f => f.Lorem.Word())
            .RuleFor(x => x.Level5, f => f.Lorem.Word())
            .RuleFor(x => x.Answers, answers ?? [])
            .RuleFor(x => x.Notes, notes ?? [])
            .RuleFor(x => x.FollowUpStatus, f => f.PickRandom(_citizenReportFollowUpStatuses))
            .RuleFor(x => x.Attachments, attachments ?? []);

        return fakeCitizenReport.Generate();
    }

    public static CitizenReportModel CitizenReport(Guid formId, BaseAnswer answer, NoteModel[] notes,
        SubmissionAttachmentModel[] attachments)
    {
        return GenerateCitizenReport(formId, [answer], notes, attachments);
    }

    public static CitizenReportModel CitizenReport(Guid formId)
    {
        return GenerateCitizenReport(formId);
    }

    public static CitizenReportModel CitizenReport(Guid formId,
        TextAnswer textAnswer, NoteModel[] textAnswerNotes, SubmissionAttachmentModel[] textAnswerAttachments,
        DateAnswer dateAnswer, NoteModel[] dateAnswerNotes, SubmissionAttachmentModel[] dateAnswerAttachments,
        NumberAnswer numberAnswer, NoteModel[] numberAnswerNotes,
        SubmissionAttachmentModel[] numberAnswerAttachments,
        RatingAnswer ratingAnswer, NoteModel[] ratingAnswerNotes,
        SubmissionAttachmentModel[] ratingAnswerAttachments,
        SingleSelectAnswer singleSelectAnswer, NoteModel[] singleSelectAnswerNotes,
        SubmissionAttachmentModel[] singleSelectAnswerAttachments,
        MultiSelectAnswer multiSelectAnswer, NoteModel[] multiSelectAnswerNotes,
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

        NoteModel[] notes =
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

        return GenerateCitizenReport(formId, answers, notes, attachments);
    }

    public static CitizenReportModel PartialCitizenReport(Guid formId,
        (TextAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? textAnswer = null,
        (DateAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? dateAnswer = null,
        (NumberAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? numberAnswer =
            null,
        (RatingAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)? ratingAnswer =
            null,
        (SingleSelectAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)?
            singleSelectAnswer = null,
        (MultiSelectAnswer answer, NoteModel[] notes, SubmissionAttachmentModel[] attachments)?
            multiSelectAnswer = null)
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

        return GenerateCitizenReport(formId, answers.ToArray(), notes.ToArray(), attachments.ToArray());
    }
}