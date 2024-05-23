using Vote.Monitor.Hangfire.Jobs.Export.QuickReports.ReadModels;

namespace Vote.Monitor.Hangfire.Jobs.Export.QuickReports;

public class QuickReportsDataTableGenerator
{
    private readonly List<string> _header;
    private readonly List<List<object>> _dataTable;
    private readonly List<QuickReportData> _quickReports = [];

    private QuickReportsDataTableGenerator(List<string> header, List<List<object>> dataTable)
    {
        _header = header;
        _dataTable = dataTable;
    }

    internal static QuickReportsDataTableGenerator For(List<string> header, List<List<object>> dataTable)
    {
        return new QuickReportsDataTableGenerator(header, dataTable);
    }

    public QuickReportsDataTableGenerator For(QuickReportModel quickReport)
    {
        MapQuickReport(quickReport);

        var row = new List<object>
        {
           quickReport.Id.ToString(),
           quickReport.Timestamp.ToString("s"),
           quickReport.FollowUpStatus.Value,
           quickReport.MonitoringObserverId.ToString(),
           quickReport.FirstName,
           quickReport.LastName,
           quickReport.Email,
           quickReport.PhoneNumber,
           quickReport.QuickReportLocationType.Value,
           quickReport.PollingStationId?.ToString() ?? "",
           quickReport.Level1 ?? "",
           quickReport.Level2 ?? "",
           quickReport.Level3 ?? "",
           quickReport.Level4 ?? "",
           quickReport.Level5 ?? "",
           quickReport.Number ?? "",
           quickReport.PollingStationDetails ?? "",
           quickReport.Title,
           quickReport.Description
        };

        _dataTable.Add(row);
        return this;
    }

    public QuickReportsDataTableGenerator ForQuickReports(List<QuickReportModel> quickReports)
    {
        foreach (var quickReport in quickReports)
        {
            For(quickReport);
        }

        return this;
    }

    private void MapQuickReport(QuickReportModel quickReport)
    {
        QuickReportData quickReportData = QuickReportData.For(quickReport);

        _quickReports.Add(quickReportData);
    }

    public (List<string> header, List<List<object>> dataTable) Please()
    {
        // get the longest column headers
        var longestAttachmentUrlsColumnHeader = _quickReports
            .MaxBy(x => x.AttachmentUrlsColumnHeaders.Count)
            ?.AttachmentUrlsColumnHeaders ?? [];

        _header.AddRange(longestAttachmentUrlsColumnHeader);

        for (var i = 0; i < _quickReports.Count; i++)
        {
            var quickReport = _quickReports[i];
            var row = _dataTable[i];

            var attachmentsUrls = PadListToTheRight(quickReport.AttachmentUrls, longestAttachmentUrlsColumnHeader.Count, string.Empty);

            row.AddRange(attachmentsUrls);
        }

        return (_header, _dataTable);
    }

    private List<T> PadListToTheRight<T>(List<T> list, int desiredLength, T padValue)
    {
        if (list.Count < desiredLength)
        {
            var numberOfMissingElements = desiredLength - list.Count;
            for (var i = 0; i < numberOfMissingElements; i++)
            {
                list.Add(padValue);
            }
        }

        return list;
    }

    internal class QuickReportData
    {
        public Guid QuickReportId { get; }
        public List<string> AttachmentUrls { get; } = [];
        public List<string> AttachmentUrlsColumnHeaders { get; } = [];

        private QuickReportData(QuickReportModel quickReport)
        {
            QuickReportId = quickReport.Id;
            AttachmentUrls = [];
            AttachmentUrlsColumnHeaders = [];
        }

        public static QuickReportData For(QuickReportModel quickReport)
        {
            var quickReportData = new QuickReportData(quickReport);

            quickReportData.WithAttachments(quickReport.Attachments);

            return quickReportData;
        }

        private void WithAttachments(QuickReportAttachmentModel[] attachments)
        {
            for (var i = 0; i < attachments.Length; i++)
            {
                AttachmentUrlsColumnHeaders.Add($"Attachment {i + 1}");
                AttachmentUrls.Add(attachments[i].PresignedUrl);
            }
        }
    }
}
