using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.ObserverGuide;

public record ObserverGuideModel
{
    public required Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? FileName { get; init; } = string.Empty;
    public string? MimeType { get; init; } = string.Empty;
    public string? PresignedUrl { get; init; } = string.Empty;
    public int? UrlValidityInSeconds { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? Text { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<ObserverGuideType, string>))]
    public ObserverGuideType GuideType { get; init; }

    public DateTime CreatedOn { get; init; }
    public string CreatedBy { get; init; }
}