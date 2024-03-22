﻿using Ardalis.SmartEnum;

namespace Vote.Monitor.Core.Security;

public sealed class UserRole : SmartEnum<UserRole, string>
{
    public static readonly UserRole PlatformAdmin = new(nameof(PlatformAdmin), nameof(PlatformAdmin));
    public static readonly UserRole NgoAdmin = new(nameof(NgoAdmin), nameof(NgoAdmin));
    public static readonly UserRole Observer = new(nameof(Observer), nameof(Observer));

    /// <summary>Gets an item associated with the specified value. Parses SmartEnum when used as query params</summary>
    /// <see href="https://github.com/ardalis/SmartEnum/issues/410#issuecomment-1686057067">this issue</see>
    /// <param name="value">The value of the item to get.</param>
    /// <param name="result">
    /// When this method returns, contains the item associated with the specified value, if the value is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="UserRole" /> contains an item with the specified name; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParse(string value, out UserRole result)
    {
        return TryFromValue(value, out result);
    }

    private UserRole(string name, string value) : base(name, value)
    {
    }
}
