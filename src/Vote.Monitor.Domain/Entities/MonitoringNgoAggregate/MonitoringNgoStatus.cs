﻿using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

public sealed class MonitoringNgoStatus : SmartEnum<MonitoringNgoStatus, string>
{
    public static readonly MonitoringNgoStatus Active = new(nameof(Active), nameof(Active));
    public static readonly MonitoringNgoStatus Suspended = new(nameof(Suspended), nameof(Suspended));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="MonitoringNgoStatus" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out MonitoringNgoStatus result)
    {
        return TryFromValue(value, out result);
    }

    private MonitoringNgoStatus(string name, string value) : base(name, value)
    {
    }
}
