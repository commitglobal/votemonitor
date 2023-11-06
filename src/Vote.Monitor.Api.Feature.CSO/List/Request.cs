﻿using System.ComponentModel;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;

namespace Vote.Monitor.Api.Feature.CSO.List;

public class Request
{
    [QueryParam]
    public string? NameFilter { get; set; }

    [QueryParam]
    [JsonConverter(typeof(SmartEnumNameConverter<CSOStatus, string>))]
    public CSOStatus? Status { get; set; }

    [QueryParam]
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    [QueryParam]
    [DefaultValue(100)]
    public int PageSize { get; set; } = 100;
}
