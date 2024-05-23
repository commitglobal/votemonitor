using Bogus;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;
using Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;
using FormAggregate = Vote.Monitor.Domain.Entities.FormAggregate.Form;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData;

public sealed class Fake
{
    private static SubmissionFollowUpStatus[] _submissionFollowUpStatuses =
    [
        SubmissionFollowUpStatus.NeedsFollowUp,
        SubmissionFollowUpStatus.NotApplicable,
        SubmissionFollowUpStatus.Resolved
    ]; 
    
    private static QuickReportFollowUpStatus[] _quickReportsFollowUpStatuses =
    [
        QuickReportFollowUpStatus.NeedsFollowUp,
        QuickReportFollowUpStatus.NotApplicable,
        QuickReportFollowUpStatus.Resolved
    ];
    public static SubmissionModel Submission(Guid formId, BaseAnswer[] answers, NoteModel[] notes, SubmissionAttachmentModel[] attachments)
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
            .RuleFor(x=>x.FollowUpStatus, f => f.PickRandom(_submissionFollowUpStatuses))
            .RuleFor(x => x.Attachments, attachments);

        return fakeSubmission.Generate();
    }

    public static QuickReportModel QuickReport(Guid quickReportId, QuickReportLocationType locationType, QuickReportAttachmentModel[] attachments)
    {
        var fakeSubmission = new Faker<QuickReportModel>()
            .RuleFor(x => x.Id, quickReportId)
            .RuleFor(x => x.Timestamp, f => f.Date.Recent(1, DateTime.UtcNow))
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.MonitoringObserverId, f => f.Random.Guid())
            .RuleFor(x => x.Attachments, attachments)
            .RuleFor(x => x.Title, f => f.Lorem.Sentence(10))
            .RuleFor(x => x.Description, f => f.Lorem.Sentence(100))
            .RuleFor(x => x.FollowUpStatus, f => f.PickRandom(_quickReportsFollowUpStatuses))
            .Rules((f, x) =>
            {
                x.QuickReportLocationType = locationType;

                if (locationType == QuickReportLocationType.VisitedPollingStation)
                {
                    x.PollingStationId = f.Random.Guid();

                    x.Level1 = f.Lorem.Word();
                    x.Level2 = f.Lorem.Word();
                    x.Level3 = f.Lorem.Word();
                    x.Level4 = f.Lorem.Word();
                    x.Level5 = f.Lorem.Word();
                    x.Number = f.Lorem.Word();
                }

                if (locationType == QuickReportLocationType.OtherPollingStation)
                {
                    x.PollingStationDetails = f.Lorem.Sentence(10);
                }
            });


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


    public static FormAggregate Form(string defaultLanguage, params BaseQuestion[] questions)
    {
        return FormAggregate.Create(Guid.NewGuid(), Guid.NewGuid(), FormType.Opening, "F1", new TranslatedString(), new TranslatedString(), defaultLanguage, [], questions);
    }
}
