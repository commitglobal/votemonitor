using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Models;

public record QuickReportDetailedModel : QuickReportOverviewModel
{
    public Guid MonitoringObserverId { get; init; }

    public List<QuickReportAttachmentModel> Attachments { get; init; }

    public static QuickReportDetailedModel FromEntity(QuickReport quickReport,
        IEnumerable<QuickReportAttachmentModel> attachments)
    {
        var attachmentsList = attachments.ToList();

        return new QuickReportDetailedModel
        {
            Id = quickReport.Id,
            QuickReportLocationType = quickReport.QuickReportLocationType,
            IssueType = quickReport.IssueType,
            OfficialComplaintFilingStatus = quickReport.OfficialComplaintFilingStatus,
            Title = quickReport.Title,
            Description = quickReport.Description,
            MonitoringObserverId = quickReport.MonitoringObserverId,
            FirstName = quickReport.MonitoringObserver.Observer.ApplicationUser.FirstName,
            LastName = quickReport.MonitoringObserver.Observer.ApplicationUser.LastName,
            PollingStationId = quickReport.PollingStationId,
            Level1 = quickReport.PollingStation?.Level1,
            Level2 = quickReport.PollingStation?.Level2,
            Level3 = quickReport.PollingStation?.Level3,
            Level4 = quickReport.PollingStation?.Level4,
            Level5 = quickReport.PollingStation?.Level5,
            Number = quickReport.PollingStation?.Number,
            Address = quickReport.PollingStation?.Address,
            PollingStationDetails = quickReport.PollingStationDetails,
            Timestamp = quickReport.LastModifiedOn ?? quickReport.CreatedOn,
            Attachments = attachmentsList,
            FollowUpStatus = quickReport.FollowUpStatus,
            NumberOfAttachments = attachmentsList.Count,
            Email = quickReport.MonitoringObserver.Observer.ApplicationUser.Email ?? "",
            PhoneNumber = quickReport.MonitoringObserver.Observer.ApplicationUser.PhoneNumber ?? "",
        };
    }
}