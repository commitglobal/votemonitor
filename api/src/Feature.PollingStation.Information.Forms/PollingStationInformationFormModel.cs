using System.Text.Json.Serialization;
using Module.Forms.Models;
using Vote.Monitor.Domain;

namespace Feature.PollingStation.Information.Forms;

public record PollingStationInformationFormModel
{
    public Guid Id { get; init; }
    public required DateTime CreatedOn { get; init; }
    /// <summary>
    /// When this version was saved; null when the form was never changed after it was created (see
    /// <see cref="CreatedOn"/>), as the mobile app has always read it.
    /// </summary>
    public required DateTime? LastModifiedOn { get; init; }

    /// <summary>The record's version (opaque); sent back with a save (concurrent edits contract).</summary>
    public uint Version { get; init; }

    /// <summary>The display name of who saved this version; null when unknown.</summary>
    public string? LastModifiedByName { get; init; }

    /// <summary>Who saved this version, to look up <see cref="LastModifiedByName"/>.</summary>
    [JsonIgnore]
    public Guid LastModifiedById { get; init; }
    public List<BaseQuestionModel> Questions { get; init; } = [];
    public string DefaultLanguage { get; init; }
    public List<string> Languages { get; init; } = [];

    /// <summary>
    /// How many polling station information records the caller's NGO's observers sent, as the forms list counts its
    /// forms; null when an NGO user didn't ask (platform admins, observers).
    /// </summary>
    public int? NumberOfSubmissions { get; init; }

    public static PollingStationInformationFormModel FromEntity(PollingStationInfoFormAggregate entity) => new()
    {
        Id = entity.Id,
        DefaultLanguage = entity.DefaultLanguage,
        Languages = entity.Languages.ToList(),
        CreatedOn = entity.CreatedOn,
        LastModifiedOn = entity.LastModifiedOn,
        Version = entity.Version,
        LastModifiedById = entity.LastModifiedBy,
        Questions = entity.Questions.Select(QuestionsMapper.ToModel).ToList()
    };

    /// <summary>The model with the display name of who saved this version.</summary>
    public async Task<PollingStationInformationFormModel> WithLastModifiedByNameAsync(IUserDisplayNameProvider userDisplayNames,
        CancellationToken ct) =>
        this with { LastModifiedByName = await userDisplayNames.GetDisplayNameAsync(LastModifiedById, ct) };
}
