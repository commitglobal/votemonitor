using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.List;

public class QuickReportOverviewModel
{
    public Guid Id { get; set; }
    public QuickReportLocationType QuickReportLocationType { get; set; }
    public DateTime Timestamp { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int NumberOfAttachments { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? PollingStationDetails { get; set; }
    public Guid? PollingStationId { get; set; }
    public string? Level1 { get; set; }
    public string? Level2 { get; set; }
    public string? Level3 { get; set; }
    public string? Level4 { get; set; }
    public string? Level5 { get; set; }
    public string? Number { get; set; }
    public string? Address { get; set; }
}
