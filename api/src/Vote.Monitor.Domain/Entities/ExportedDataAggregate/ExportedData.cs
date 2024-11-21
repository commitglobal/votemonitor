using Vote.Monitor.Domain.Entities.ExportedDataAggregate.Filters;

namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate;

public class ExportedData : BaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public ExportedDataType ExportedDataType { get; private set; }
    public ExportedDataStatus ExportStatus { get; private set; }
    public string? FileName { get; private set; }
    public string? Base64EncodedData { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public ExportFormSubmissionsFilters? FormSubmissionsFilters { get; private set; }
    public ExportQuickReportsFilters? QuickReportsFilters { get; private set; }
    public ExportCitizenReportsFilers? CitizenReportsFilers { get; private set; }
    public ExportIncidentReportsFilters? IncidentReportsFilters { get; private set; }

    private ExportedData(Guid electionRoundId,
        ExportedDataType exportedDataType,
        DateTime startedAt,
        ExportFormSubmissionsFilters? formSubmissionsFilters,
        ExportQuickReportsFilters? quickReportsFilters,
        ExportCitizenReportsFilers? citizenReportsFilers,
        ExportIncidentReportsFilters? incidentReportsFilters) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        ExportStatus = ExportedDataStatus.Started;
        StartedAt = startedAt;
        ExportedDataType = exportedDataType;
        FormSubmissionsFilters = formSubmissionsFilters;
        QuickReportsFilters = quickReportsFilters;
        CitizenReportsFilers = citizenReportsFilers;
        IncidentReportsFilters = incidentReportsFilters;
    }

    public static ExportedData Create(Guid electionRoundId, ExportedDataType dataType, DateTime startedAt)
    {
        return new ExportedData(electionRoundId: electionRoundId,
            exportedDataType: dataType,
            startedAt: startedAt,
            formSubmissionsFilters: null,
            quickReportsFilters: null,
            citizenReportsFilers: null,
            incidentReportsFilters: null);
    }

    public void Fail()
    {
        ExportStatus = ExportedDataStatus.Failed;
    }

    public void Complete(string fileName, string base64EncodedData, DateTime completedAt)
    {
        FileName = fileName;
        Base64EncodedData = base64EncodedData;
        ExportStatus = ExportedDataStatus.Completed;
        CompletedAt = completedAt;
    }

    public static ExportedData CreateForFormSubmissions(Guid electionRoundId, ExportedDataType dataType,
        DateTime startedAt, ExportFormSubmissionsFilters? filters)
    {
        return new ExportedData(electionRoundId: electionRoundId,
            exportedDataType: dataType,
            startedAt: startedAt,
            formSubmissionsFilters: filters,
            quickReportsFilters: null,
            citizenReportsFilers: null,
            incidentReportsFilters: null);
    }

    public static ExportedData CreateForQuickReports(Guid electionRoundId, ExportedDataType dataType,
        DateTime startedAt, ExportQuickReportsFilters? filters)
    {
        return new ExportedData(electionRoundId: electionRoundId,
            exportedDataType: dataType,
            startedAt: startedAt,
            formSubmissionsFilters: null,
            quickReportsFilters: filters,
            citizenReportsFilers: null,
            incidentReportsFilters: null);
    }

    public static ExportedData CreateForCitizenReports(Guid electionRoundId, ExportedDataType dataType,
        DateTime startedAt, ExportCitizenReportsFilers? filters)
    {
        return new ExportedData(electionRoundId: electionRoundId,
            exportedDataType: dataType,
            startedAt: startedAt,
            formSubmissionsFilters: null,
            quickReportsFilters: null,
            citizenReportsFilers: filters,
            incidentReportsFilters: null);
    }

    public static ExportedData CreateForIncidentReports(Guid electionRoundId, ExportedDataType dataType,
        DateTime startedAt, ExportIncidentReportsFilters? filters)
    {
        return new ExportedData(electionRoundId: electionRoundId,
            exportedDataType: dataType,
            startedAt: startedAt,
            formSubmissionsFilters: null,
            quickReportsFilters: null,
            citizenReportsFilers: null,
            incidentReportsFilters: filters);
    }


#pragma warning disable CS8618 // Required by Entity Framework
    private ExportedData()
    {
    }
#pragma warning restore CS8618
}
