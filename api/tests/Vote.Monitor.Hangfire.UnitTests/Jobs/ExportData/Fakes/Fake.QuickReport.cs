using Bogus;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

namespace Vote.Monitor.Hangfire.UnitTests.Jobs.ExportData.Fakes;

public sealed partial class Fake
{
    public static QuickReportModel QuickReport(Guid quickReportId, QuickReportLocationType locationType,
        QuickReportAttachmentModel[] attachments)
    {
        var fakeQuickReport = new Faker<QuickReportModel>()
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
            .RuleFor(x => x.FollowUpStatus,
                f => f.PickRandom<QuickReportFollowUpStatus>(QuickReportFollowUpStatus.List))
            .RuleFor(x => x.IncidentCategory, f => f.PickRandom<IncidentCategory>(IncidentCategory.List))
            .Rules((f, x) =>
            {
                x.QuickReportLocationType = locationType;

                if (locationType == QuickReportLocationType.VisitedPollingStation)
                {
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


        return fakeQuickReport.Generate();
    }
}