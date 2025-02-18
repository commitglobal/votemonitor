﻿namespace Vote.Monitor.Domain.Entities.ExportedDataAggregate;

[JsonConverter(typeof(SmartEnumValueConverter<ExportedDataType, string>))]
public sealed class ExportedDataType : SmartEnum<ExportedDataType, string>
{
    public static readonly ExportedDataType FormSubmissions = new(nameof(FormSubmissions), nameof(FormSubmissions));
    public static readonly ExportedDataType QuickReports = new(nameof(QuickReports), nameof(QuickReports));
    public static readonly ExportedDataType PollingStations = new(nameof(PollingStations), nameof(PollingStations));
    public static readonly ExportedDataType CitizenReports = new(nameof(CitizenReports), nameof(CitizenReports));
    public static readonly ExportedDataType Locations = new(nameof(Locations), nameof(Locations));
    public static readonly ExportedDataType IncidentReports = new(nameof(IncidentReports), nameof(IncidentReports));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="ExportedDataType" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out ExportedDataType result)
    {
        return TryFromValue(value, out result);
    }

    [JsonConstructor]
    private ExportedDataType(string name, string value) : base(name, value)
    {
    }
}
