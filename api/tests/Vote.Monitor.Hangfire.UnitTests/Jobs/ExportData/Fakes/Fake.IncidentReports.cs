using Bogus;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Hangfire.Jobs.Export.IncidentReports.ReadModels;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    private static readonly IncidentReportFollowUpStatus[] _incidentReportFollowUpStatuses =
    [
        IncidentReportFollowUpStatus.NeedsFollowUp,
        IncidentReportFollowUpStatus.NotApplicable,
        IncidentReportFollowUpStatus.Resolved
    ];
    
    private static readonly IncidentReportLocationType[] _incidentReportLocationTypes =
    [
        IncidentReportLocationType.PollingStation,
        IncidentReportLocationType.OtherLocation,
    ];

    private static IncidentReportModel GenerateIncidentReport(
        Guid formId,
        IncidentReportLocationType? locationType = null,
        BaseAnswer[]? answers = null,
        SubmissionNoteModel[]? notes = null,
        SubmissionAttachmentModel[]? attachments = null)
    {
        var fakeIncidentReport = new Faker<IncidentReportModel>()
            .RuleFor(x => x.FormId, formId)
            .RuleFor(x => x.IncidentReportId, f => f.Random.Guid())
            .RuleFor(x => x.TimeSubmitted, f => f.Date.Recent(1, DateTime.UtcNow))
       
            .Rules((f, x) =>
            {
                x.LocationType = locationType ?? f.PickRandom(_incidentReportLocationTypes);

                if (locationType == IncidentReportLocationType.PollingStation)
                {
                    x.Level1 = f.Lorem.Word();
                    x.Level2 = f.Lorem.Word();
                    x.Level3 = f.Lorem.Word();
                    x.Level4 = f.Lorem.Word();
                    x.Level5 = f.Lorem.Word();
                    x.Number = f.Lorem.Word();
                }

                if (locationType == IncidentReportLocationType.OtherLocation)
                {
                    x.LocationDescription = f.Lorem.Sentence(10);
                }
            })
            .RuleFor(x => x.NgoName, f => f.Company.CompanyName())
            .RuleFor(x => x.DisplayName, f => f.Name.FirstName())
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.MonitoringObserverId, f => f.Random.Guid())
            
            .RuleFor(x => x.Answers, answers ?? [])
            .RuleFor(x => x.Notes, notes ?? [])
            .RuleFor(x => x.FollowUpStatus, f => f.PickRandom(_incidentReportFollowUpStatuses))
            .RuleFor(x => x.Attachments, attachments ?? []);

        return fakeIncidentReport.Generate();
    }

    public static IncidentReportModel IncidentReport(Guid formId,
        IncidentReportLocationType locationType,
        BaseAnswer answer,
        SubmissionNoteModel[] notes,
        SubmissionAttachmentModel[] attachments)
    {
        return GenerateIncidentReport(formId, locationType,[answer], notes, attachments);
    }

    public static IncidentReportModel IncidentReport(Guid formId)
    {
        return GenerateIncidentReport(formId);
    }

    public static IncidentReportModel IncidentReport(Guid formId,
        IncidentReportLocationType locationType,
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

        return GenerateIncidentReport(formId, locationType, answers, notes, attachments);
    }

    public static IncidentReportModel PartialIncidentReport(Guid formId,
        IncidentReportLocationType locationType,
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

        return GenerateIncidentReport(formId, locationType, answers.ToArray(), notes.ToArray(), attachments.ToArray());
    }
}
