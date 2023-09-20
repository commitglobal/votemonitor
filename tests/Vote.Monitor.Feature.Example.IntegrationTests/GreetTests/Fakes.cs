using Bogus;
using Greet = Vote.Monitor.Feature.Example.Greet;

namespace Tests.GreetTests;

static class Fakes
{
    public static Greet.Request GreetRequest(this Faker f) => new()
    {
        Name = f.Name.FullName()
    };
}
