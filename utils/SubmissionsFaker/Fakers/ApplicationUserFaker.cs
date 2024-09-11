using Bogus;
using SubmissionsFaker.Clients.PlatformAdmin.Models;

namespace SubmissionsFaker.Fakers;

public sealed class ApplicationUserFaker : Faker<ApplicationUser>
{
    public ApplicationUserFaker(string? email = null, string? password = null)
    {
        RuleFor(x => x.Email, f => email ?? f.Internet.Email(provider: "example.com"));
        RuleFor(x => x.Password, f => password ?? "string");
        RuleFor(x => x.FirstName, f => f.Name.FirstName());
        RuleFor(x => x.LastName, f => f.Name.LastName());
        RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber());
    }
}