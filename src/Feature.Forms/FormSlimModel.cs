using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms;

public class FormSlimModel
{
    public static FormSlimModel FromEntity(FormAggregate form) => form == null ? null : new FormSlimModel
    {
        Id = form.Id,
        Code = form.Code,
        FormType = form.FormType,
        Status = form.Status,
        DefaultLanguage = form.DefaultLanguage,
        Languages = form.Languages,
        Name = form.Name,
        CreatedOn = form.CreatedOn,
        LastModifiedOn = form.LastModifiedOn
    };

    public Guid Id { get; init; }
    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public FormType FormType { get; init; }
    public string Code { get; init; }
    public TranslatedString Name { get; init; }
    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public FormStatus Status { get; init; }
    public string DefaultLanguage { get; init; }
    public string[] Languages { get; init; } = [];
    public DateTime CreatedOn { get; init; }
    public DateTime? LastModifiedOn { get; init; }
}
