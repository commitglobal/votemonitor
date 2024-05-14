using Bogus;
using SubmissionsFaker.Clients.PlatformAdmin.Models;

namespace SubmissionsFaker.Fakers;

public sealed class NgoFaker : Faker<Ngo>
{
    public NgoFaker()
    {
        RuleFor(x => x.Name, f => f.Internet.UserName());
    }
}