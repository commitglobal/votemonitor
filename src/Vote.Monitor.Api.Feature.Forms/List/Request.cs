using System.ComponentModel;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Forms.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [QueryParam]
    public string? CodeFilter { get; set; }

    [QueryParam]
    public Guid? LanguageId { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public FormStatus? Status { get; set; }

    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;
}
