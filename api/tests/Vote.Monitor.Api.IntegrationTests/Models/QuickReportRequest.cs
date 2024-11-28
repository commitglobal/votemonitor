using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Api.IntegrationTests.Models;

public class QuickReportRequest
{
    public Guid Id { get; set; }
    public QuickReportLocationType QuickReportLocationType =>QuickReportLocationType.VisitedPollingStation;
    public IncidentCategory IncidentCategory => IncidentCategory.Other;
    
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid PollingStationId { set; get; }
}
