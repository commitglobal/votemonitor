using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;
public class PollingStationRepositoryDeleteTests
{
    private List<PollingStationModel> _pollingStations = new List<PollingStationModel>
            {
                new PollingStationModel
                {
                    Id = 1,
                    Address ="addr1",
                    DisplayOrder =0,
                    Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key1", Value = "value1"},
                        new TagModel {Key = "key2", Value = "value2"},
                        new TagModel {Key = "ke3", Value = "value3"}
                    }
                },
                new PollingStationModel
                {
                    Id = 2,
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key1", Value = "value2"},
                        new TagModel {Key = "key2", Value = "value3"},
                        new TagModel {Key = "ke3", Value = "value3"}
                    }
                }

            };

    private void Init(string dbname, out AppDbContext context, out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
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

        var id = 3;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<PollingStationModel>>(() => repository.DeleteAsync(id));
    }

    [Fact]
    public async Task DeleteAllAsync_ShouldDeleteAllPollingStations()
    {
        // Arrange
        Init("delTest2", out AppDbContext context, out PollingStationRepository repository);
        // Act
        await repository.DeleteAllAsync();

        // Assert
        Assert.True(context.PollingStations.Count() == 0, "PollingStations collection is not empty");
    }
    [Fact]
    public async Task DeleteAsync_ShouldDeletePollingStation()
    {
        // Arrange
        Init("delTest3", out AppDbContext context, out PollingStationRepository repository);
        var id = 1;


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
        var pollingStationId = 3;
        var pollingStation = new PollingStationModel
        {
            Id = pollingStationId,
            DisplayOrder = 3,
            Address = "123 Main St",
            Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key test", Value = "value test"},
                    }
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

        var pollingStationId = 4;
        var pollingStation = new PollingStationModel
        {
            Id = pollingStationId,
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new List<TagModel>
        {
            new TagModel { Key = "key1", Value = "value one" },
            new TagModel { Key = "key2", Value = "value two" }
        }
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
