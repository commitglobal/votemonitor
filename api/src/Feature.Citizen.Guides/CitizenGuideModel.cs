using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;

namespace Feature.Citizen.Guides;

public record CitizenGuideModel
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string FileName { get; init; }
    public string MimeType { get; init; }
    public string GuideType { get; init; }
    public string Text { get; init; }
    public string WebsiteUrl { get; init; }
    public DateTime CreatedOn { get; init; }
    public string CreatedBy { get; init; }
    public string PresignedUrl { get; init; }
    public int UrlValidityInSeconds { get; init; }
    public string FilePath { get; init; }
    public string UploadedFileName { get; init; }
    public bool IsGuideOwner => true;
}
