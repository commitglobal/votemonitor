﻿using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.Forms.List;

public class Request: BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    [QueryParam]
    public string? CodeFilter { get; set; }

    [QueryParam]
    public string? LanguageCode { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<FormTemplateStatus, string>))]
    public FormTemplateStatus? Status { get; set; }
}
