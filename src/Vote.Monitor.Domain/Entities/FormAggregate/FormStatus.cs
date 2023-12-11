using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public sealed class FormStatus : SmartEnum<FormStatus, string>
{
    /// <summary>
    /// The form is being created or edited but has not been finalized for distribution.
    /// </summary>
    public static readonly FormStatus Draft = new(nameof(Draft), nameof(Draft));

    /// <summary>
    /// The form is live and available for respondents to complete.
    /// </summary>
    public static readonly FormStatus InProgress = new(nameof(InProgress), nameof(InProgress));

    /// <summary>
    /// The collection of forms has completed, and responses are awaiting analysis.
    /// </summary>
    public static readonly FormStatus Completed = new(nameof(Completed), nameof(Completed));

    /// <summary>
    /// The form and its data are stored for historical purposes but are not actively used.
    /// </summary>
    public static readonly FormStatus Archived = new(nameof(Archived), nameof(Archived));

    private FormStatus(string name, string value) : base(name, value)
    {
    }
}
