﻿using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

public class RatingQuestionRequest : BaseQuestionRequest
{
    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; set; }
}
