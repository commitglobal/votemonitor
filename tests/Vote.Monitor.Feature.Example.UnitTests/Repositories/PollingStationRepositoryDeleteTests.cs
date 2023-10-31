using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;
public class PollingStationRepositoryDeleteTests
{
    private readonly List<Domain.Models.PollingStation> _pollingStations = new()
            {
                new Domain.Models.PollingStation
                {
                    Id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1"),
                    Address ="addr1",
                    DisplayOrder =0,
                    Tags = new Dictionary<string, string>()
                    {
                         {  "key1",  "value1"},
                         {  "key2",  "value2"},
                         {  "ke3",  "value3"}
                    }.ToTags()
                },
                new Domain.Models.PollingStation
                {
                    Id = Guid.Parse("7ea27240-0711-49de-98fb-1d3e9df2fffd"),
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new Dictionary<string, string>()
                    {
                         {  "key1",  "value2"},
                         {  "key2",  "value3"},
                         {  "ke3",  "value3"}
                    }.ToTags()
                }
                }

            };

    private void Init(string dbname, out AppDbContext context, out PollingStationRepository repository)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(dbname);
        context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);
        repository.AddAsync(_pollingStations[0]).Wait();
        repository.AddAsync(_pollingStations[1]).Wait();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        Init("delTest1", out AppDbContext context, out PollingStationRepository repository);

        var id = Guid.Parse("afb6e443-9e37-4c9f-b737-f38352ffbfec");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<Domain.Models.PollingStation>>(() => repository.DeleteAsync(id));
    }

    [Fact]
    public async Task DeleteAllAsync_ShouldDeleteAllPollingStations()
    {
        // Arrange
        Init("delTest2", out AppDbContext context, out PollingStationRepository repository);
        // Act
        await repository.DeleteAllAsync();

        // Assert
        Assert.True(!context.PollingStations.Any(), "PollingStations collection is not empty");
    }
    [Fact]
    public async Task DeleteAsync_ShouldDeletePollingStation()
    {
        // Arrange
        Init("delTest3", out AppDbContext context, out PollingStationRepository repository);
        var id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1");


        // Act
        await repository.DeleteAsync(id);

        // Assert

        Assert.True(context.PollingStations.Count() == 1, "PollingStation not deleted");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePollingStationAndOrphanedTags()
    {
        Init("delTest4", out AppDbContext context, out PollingStationRepository repository);

        // Arrange
        var pollingStationId = Guid.Parse("0ff0ec80-bf09-4154-833c-6dd9f3c579cd");
        var pollingStation = new Domain.Models.PollingStation
        {
            Id = pollingStationId,
            DisplayOrder = 3,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
                    {
                         {  "key test",  "value test"},
                    }.ToTags()
        };
        await repository.AddAsync(pollingStation);

        // Act
        await repository.DeleteAsync(pollingStationId);

        // Assert
        var deletedPollingStation = await context.PollingStations.FirstOrDefaultAsync(ps => ps.Id == pollingStationId);
        Assert.Null(deletedPollingStation);

        var deletedOrphanedTag = await context.Tags.FirstOrDefaultAsync(tag => tag.Key == "key test");
        Assert.Null(deletedOrphanedTag);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotDeletePollingStationWithTags()
    {
        // Arrange
        Init("delTest5", out AppDbContext context, out PollingStationRepository repository);

        var pollingStationId = Guid.Parse("0ff0ec80-bf09-4154-833c-6dd9f3c579cd"); ;
        var pollingStation = new Domain.Models.PollingStation
        {
            Id = pollingStationId,
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
        {
             {  "key1",  "value one" },
             {  "key2",  "value two" }
        }.ToTags()
        };
        await repository.AddAsync(pollingStation);

        // Act
        await repository.DeleteAsync(pollingStationId);

        // Assert
        var deletedPollingStation = await context.PollingStations.FirstOrDefaultAsync(ps => ps.Id == pollingStationId);
        Assert.Null(deletedPollingStation);

        var existingTag1 = await context.Tags.FirstOrDefaultAsync(tag => tag.Key == "key1");
        Assert.NotNull(existingTag1);

        var existingTag2 = await context.Tags.FirstOrDefaultAsync(tag => tag.Key == "key2");
        Assert.NotNull(existingTag2);
    }
}
