using Bogus;
using SubmissionsFaker.Clients.PlatformAdmin;

namespace SubmissionsFaker.Fakers;

public sealed class ApplicationUserFaker : Faker<ApplicationUser>
{
    public ApplicationUserFaker()
    {
        RuleFor(x => x.Email, f => f.Internet.Email(provider: "example.com"));
        RuleFor(x => x.Password, f => f.Internet.Password());
        RuleFor(x => x.FirstName, f => f.Name.FirstName());
        RuleFor(x => x.LastName, f => f.Name.LastName());
        RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber());
    }
}