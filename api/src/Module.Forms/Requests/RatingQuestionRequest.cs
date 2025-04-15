using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Forms.Requests;

public class RatingQuestionRequest : BaseQuestionRequest
{
    public TranslatedString? UpperLabel { get; set; }
    public TranslatedString? LowerLabel { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<RatingScale, string>))]
    public RatingScale Scale { get; set; }
}