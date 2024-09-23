using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenReportGuideAggregate;

namespace Feature.CitizenReports.Guides;

public record CitizenReportsGuideModel
{
    public required Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? FileName { get; init; } = string.Empty;
    public string? MimeType { get; init; } = string.Empty;
    public string? PresignedUrl { get; init; } = string.Empty;
    public int? UrlValidityInSeconds { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? Text { get; init; }

    [JsonConverter(typeof(SmartEnumNameConverter<CitizenReportGuideType, string>))]
    public CitizenReportGuideType GuideType { get; init; }

    public DateTime CreatedOn { get; init; }
    public string CreatedBy { get; init; }
}