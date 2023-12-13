using System;

namespace Vote.Monitor.Api.Feature.Observer.Services;
//implement IEquatable interface

public class ObserverImportModel 

{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string Password { get => "string"; }
    public required string PhoneNumber { get; set; }

   
    public override int GetHashCode()
    {
        return Email.GetHashCode();
    }

}





