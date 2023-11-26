using System;
using AutoBogus;
using Bogus;
using Bogus.DataSets;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Specifications;

public class ObserverAggregateFaker : AutoFaker<ObserverAggregate>
{
    UserStatus[] statuses = new UserStatus[] { UserStatus.Active, UserStatus.Deactivated };
    public ObserverAggregateFaker(string? name=null, string? login=null, string? password=null, UserStatus? status=null)
    {
        RuleFor(fake => fake.Name, fake => name ?? fake.Name.FirstName());
        RuleFor(fake => fake.Login, fake => login ?? fake.Internet.Email());
        RuleFor(fake => fake.Password, fake => password ?? fake.Random.String2(10));
        RuleFor(fake => fake.Status, fake => status ?? statuses[fake.Random.Int(0, 1)]);

    }
}