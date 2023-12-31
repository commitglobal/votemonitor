﻿namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public abstract class ApplicationUser : BaseEntity, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected ApplicationUser()
    {

    }

    public string Name { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }

    public ApplicationUser(string name, string login, string password, UserRole role)
    {
        Name = name;
        Login = login;
        Password = password;
        Role = role;
        Status = UserStatus.Active;
    }

    public void UpdateDetails(string name)
    {
        Name = name;
    }

    public void Activate()
    {
        // TODO: handle invariants
        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        // TODO: handle invariants
        Status = UserStatus.Deactivated;
    }
}
