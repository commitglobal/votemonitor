using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;
public class PollingStationRepositoryUpdateTests
{

    private static void Init(string dbname, out AppDbContext context, out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(dbname);
        context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);

    }
    [Fact]
    public async Task UpdateAsync_ShouldUpdatePollingStation()
    {
        Init("TestDb6", out AppDbContext context, out PollingStationRepository repository);

        // Arrange
        var id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1");
        var entity = new Domain.Models.PollingStation
        {
            Id = id,
            DisplayOrder = 2,
            Address = "456 Main St",
            Tags = new Dictionary<string, string>()
                {
                     {  "key3",  "value3"},
                     {  "key4",  "value4"}
                }.ToTags()
        };
        var pollingStation = new Domain.Models.PollingStation
        {
            Id = id,
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
                {
                     {  "key1",  "value1"},
                     {  "key2",  "value2"}
                }.ToTags()
        };
        await repository.AddAsync(pollingStation);


        // Act
        var result = await repository.UpdateAsync(id, entity);

        // Assert

        Assert.Equal(entity.DisplayOrder, result.DisplayOrder);
        Assert.Equal(entity.Address, result.Address);
        Assert.True(result.Tags.ToDictionary().Any(t => t.Key == entity.Tags.ToDictionary().First().Key && t.Value == entity.Tags.ToDictionary().First().Value), "tags not found");
        Assert.True(result.Tags.ToDictionary().Any(t => t.Key == entity.Tags.ToDictionary().Last().Key && t.Value == entity.Tags.ToDictionary().Last().Value), "tags not found");
        //todo -check the tags count
        //Assert.True(context.Tags.Count() == 2, "tags count failed");    
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase("TestDb7");
        var context = new AppDbContext(optionsBuilder.Options);
        var repository = new PollingStationRepository(context);
        var id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1");
        var entity = new Domain.Models.PollingStation { Id = id, Address = "addr1" };


        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<Domain.Models.PollingStation>>(() => repository.UpdateAsync(id, entity));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOnlySpecifiedProperties()
    {
        Init("TestDb8", out _, out PollingStationRepository repository);

        // Arrange
        var id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1");
        var existingPollingStation = new Domain.Models.PollingStation
        {
            Id = id,
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
        {
             {  "key1",  "value1"},
             {  "key2",  "value2"}
        }.ToTags()
        };

        var updatedPollingStation = new Domain.Models.PollingStation
        {
            Id = id,
            DisplayOrder = 2,
            Address = "456 Main St",
            Tags = new Dictionary<string, string>(){
                         {  "key1",  "value1"},
                         {  "key2",  "value3"}
                    }.ToTags()
        };

        await repository.AddAsync(existingPollingStation);

        // Act
        var result = await repository.UpdateAsync(id, updatedPollingStation);

        // Assert
        Assert.Equal(updatedPollingStation.DisplayOrder, result.DisplayOrder);
        Assert.Equal(updatedPollingStation.Address, result.Address);
        var resultedTags = result.Tags.ToDictionary();
        Assert.Equal(2, resultedTags.Count);
        Assert.Contains(existingPollingStation.Tags.ToDictionary().First(), resultedTags);
        Assert.Contains(existingPollingStation.Tags.ToDictionary().Last(), resultedTags);
    }

    [Fact]
    public async Task UpdateAsync_ShouldClearTags()
    {
        Init("TestDb9", out _, out PollingStationRepository repository);

        // Arrange
        var id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1");
        var existingPollingStation = new Domain.Models.PollingStation
        {
            Id = id,
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
        {
             {  "key1",  "value1" },
             {  "key2",  "value2" },
        }.ToTags(),
        };
        await repository.AddAsync(existingPollingStation);

        var updatedPollingStation = new Domain.Models.PollingStation
        {
            Id = id,
            DisplayOrder = 2,
            Address = "456 Main St",
            Tags = new Dictionary<string, string>().ToTags(),
        };

        // Act
        var result = await repository.UpdateAsync(id, updatedPollingStation);

        // Assert
        Assert.Equal(updatedPollingStation.DisplayOrder, result.DisplayOrder);
        Assert.Equal(updatedPollingStation.Address, result.Address);
        Assert.Empty(result.Tags.ToDictionary());
    }
}
