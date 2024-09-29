using Bogus;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Jobs.Export.IssueReports.ReadModels;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    private static readonly IssueReportFollowUpStatus[] _issueReportFollowUpStatuses =
    [
        IssueReportFollowUpStatus.NeedsFollowUp,
        IssueReportFollowUpStatus.NotApplicable,
        IssueReportFollowUpStatus.Resolved
    ];
    
    private static readonly IssueReportLocationType[] _issueReportLocationTypes =
    [
        IssueReportLocationType.PollingStation,
        IssueReportLocationType.OtherLocation,
    ];

    private static IssueReportModel GenerateIssueReport(
        Guid formId,
        IssueReportLocationType? locationType = null,
        BaseAnswer[]? answers = null,
        SubmissionNoteModel[]? notes = null,
        SubmissionAttachmentModel[]? attachments = null)
    {
        var fakeIssueReport = new Faker<IssueReportModel>()
            .RuleFor(x => x.FormId, formId)
            .RuleFor(x => x.IssueReportId, f => f.Random.Guid())
            .RuleFor(x => x.TimeSubmitted, f => f.Date.Recent(1, DateTime.UtcNow))
       
            .Rules((f, x) =>
            {
                x.LocationType = locationType ?? f.PickRandom(_issueReportLocationTypes);

                if (locationType == IssueReportLocationType.PollingStation)
                {
                    x.Level1 = f.Lorem.Word();
                    x.Level2 = f.Lorem.Word();
                    x.Level3 = f.Lorem.Word();
                    x.Level4 = f.Lorem.Word();
                    x.Level5 = f.Lorem.Word();
                    x.Number = f.Lorem.Word();
                }

                if (locationType == IssueReportLocationType.OtherLocation)
                {
                    x.LocationDescription = f.Lorem.Sentence(10);
                }
            })
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.MonitoringObserverId, f => f.Random.Guid())
            
            .RuleFor(x => x.Answers, answers ?? [])
            .RuleFor(x => x.Notes, notes ?? [])
            .RuleFor(x => x.FollowUpStatus, f => f.PickRandom(_issueReportFollowUpStatuses))
            .RuleFor(x => x.Attachments, attachments ?? []);

        return fakeIssueReport.Generate();
    }

    public static IssueReportModel IssueReport(Guid formId,
        IssueReportLocationType locationType,
        BaseAnswer answer,
        SubmissionNoteModel[] notes,
        SubmissionAttachmentModel[] attachments)
    {
        return GenerateIssueReport(formId, locationType,[answer], notes, attachments);
    }

    public static IssueReportModel IssueReport(Guid formId)
    {
        return GenerateIssueReport(formId);
    }

    public static IssueReportModel IssueReport(Guid formId,
        IssueReportLocationType locationType,
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

        return GenerateIssueReport(formId, locationType, answers, notes, attachments);
    }

    public static IssueReportModel PartialIssueReport(Guid formId,
        IssueReportLocationType locationType,
        (TextAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? textAnswer = null,
        (DateAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? dateAnswer = null,
        (NumberAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? numberAnswer =  null,
        (RatingAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? ratingAnswer = null,
        (SingleSelectAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? singleSelectAnswer = null,
        (MultiSelectAnswer answer, SubmissionNoteModel[] notes, SubmissionAttachmentModel[] attachments)? multiSelectAnswer = null)
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

        return GenerateIssueReport(formId, locationType, answers.ToArray(), notes.ToArray(), attachments.ToArray());
    }
}