﻿using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

public class ApplicationUser : IdentityUser<Guid>, IAggregateRoot
{
#pragma warning disable CS8618 // Required by Entity Framework
    protected ApplicationUser()
    {

    }
#pragma warning restore CS8618

    public UserRole Role { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiryTime { get; private set; }
    public UserStatus Status { get; private set; }
    public UserPreferences Preferences { get; private set; }
    public Guid? InvitationToken { get; set; }

    private ApplicationUser(UserRole role, string firstName, string lastName, string email, string phoneNumber, string password)
    {
        Role = role;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        NormalizedEmail = email.ToUpperInvariant();
        NormalizedUserName = email.ToUpperInvariant();
        PhoneNumber = phoneNumber;

        Status = UserStatus.Active;
        Preferences = UserPreferences.Defaults;

        if (string.IsNullOrEmpty(password))
        {
            NewInvite();
        }
        else
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            PasswordHash = hasher.HashPassword(this, password);
        }
    }

    public static ApplicationUser Invite(string firstName, string lastName, string email, string phoneNumber) =>
        new(UserRole.Observer, firstName, lastName, email, phoneNumber, string.Empty);

    public static ApplicationUser CreatePlatformAdmin(string firstName, string lastName, string email, string phoneNumber, string password) =>
         new(UserRole.PlatformAdmin, firstName, lastName, email, phoneNumber, password); 
    
    public static ApplicationUser CreateNgoAdmin(string firstName, string lastName, string email, string phoneNumber, string password) =>
         new(UserRole.NgoAdmin, firstName, lastName, email, phoneNumber, password);
    public static ApplicationUser CreateObserver(string firstName, string lastName, string email, string phoneNumber, string password) =>
         new(UserRole.Observer, firstName, lastName, email, phoneNumber, password);

    public void UpdateDetails(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public void UpdateRefreshToken(string refreshToken, DateTime refreshTokenExpiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = refreshTokenExpiryTime;
    }
    public void AcceptInvite(string password)
    {
        InvitationToken = null;
        var hasher = new PasswordHasher<ApplicationUser>();
        PasswordHash = hasher.HashPassword(this, password);
    }


    public void NewInvite()
    {
        InvitationToken = Guid.NewGuid();
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
