using Microsoft.Extensions.Logging;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Repository;

namespace Vote.Monitor.Api.IntegrationTests.Ngo;

public class NgoDataSeeder(
    ILogger<NgoDataSeeder> logger,
    IRepository<Domain.Entities.NgoAggregate.Ngo> repository,
    ITimeProvider timeProvider)
    : IDataSeeder
{
    public async Task SeedDataAsync()
    {
        logger.LogInformation("Generating ngo Test Data");

        var activatedNgos = Enumerable
            .Range(0, 20)
            .Select(x => new Domain.Entities.NgoAggregate.Ngo($"Activated{x}"));

        var deactivatedNgos = Enumerable
                  .Range(0, 20)
                  .Select(x =>
                  {
                      var ngo = new Domain.Entities.NgoAggregate.Ngo($"Deactivated{x}");
                      ngo.Deactivate();
                      return ngo;
                  });

        await repository.AddRangeAsync(activatedNgos);
        await repository.AddRangeAsync(deactivatedNgos);
    }
}
