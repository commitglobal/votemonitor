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
        LastModifiedOn = form.LastModifiedOn,
        NumberOfQuestions = form.NumberOfQuestions,
        Description = form.Description,
    };

    public TranslatedString Description { get; set; }

    public required Guid Id { get; init; }
    [JsonConverter(typeof(SmartEnumNameConverter<FormType, string>))]
    public required FormType FormType { get; init; }
    public required string Code { get; init; }
    public TranslatedString Name { get; init; }
    [JsonConverter(typeof(SmartEnumNameConverter<FormStatus, string>))]
    public required FormStatus Status { get; init; }
    public required string DefaultLanguage { get; init; }
    public required string[] Languages { get; init; } = [];
    public int NumberOfQuestions { get; init; }
    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}
