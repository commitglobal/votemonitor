﻿using Ardalis.SmartEnum;

namespace Vote.Monitor.Domain.Entities.NotificationStubAggregate;

public class NotificationStubType : SmartEnum<NotificationStubType, string>
{
    public static readonly NotificationStubType Firebase = new(nameof(Firebase), nameof(Firebase));
    public static readonly NotificationStubType Expo = new(nameof(Expo), nameof(Expo));

    private NotificationStubType(string name, string value) : base(name, value)
    {
    }
}
