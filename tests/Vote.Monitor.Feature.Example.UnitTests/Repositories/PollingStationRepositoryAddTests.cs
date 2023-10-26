﻿using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;

public class PollingStationRepositoryAddTests
{

    [Fact]
    public async Task AddAsync_ShouldAddPollingStation()
    {
        // Arrange

        Init("addTestDb1", out AppDbContext context, out PollingStationRepository repository);



        var entity = new PollingStationModel
        {
            Id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8"),
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new List<TagModel>
            {
                new TagModel {Key = "key1" + Guid.NewGuid().ToString(), Value = "value1"},
                new TagModel {Key = "key2" + Guid.NewGuid().ToString(), Value = "value2"}
            }
        };
        var initPSCount = context.PollingStations.Count();
        var initTagCount = context.PollingStations.Count();
        // Act
        var result = await repository.AddAsync(entity);

        //Assert
        var tagCount = context.Tags.Count();
        Assert.Equal(entity, result);
        Assert.True(context.PollingStations.Count() == initPSCount + 1, "PS count failed");
        Assert.True(context.Tags.Count() == initTagCount + 2, $"Tags count failed {tagCount}, inittagcount {initTagCount}");

    }


    [Fact]
    public async Task AddAsync_ShouldThrowExceptionDuplicateTagKey()
    {
        // Arrange
        Init("addTestDb2", out _, out PollingStationRepository repository);
        var entity = new PollingStationModel
        {
            Id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8"),
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new List<TagModel>
            {
                new TagModel {Key = "key1", Value = "value1"},
                new TagModel {Key = "key1", Value = "value2"}
            }
        };

        //act 
        Task act() => repository.AddAsync(entity);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);
    }

    private static void Init(string dbname, out AppDbContext context, out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(dbname);
        context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);
    }

    [Fact]

    public async Task AddAsync_ShouldThrowExceptionNoTag()
    {

        // Arrange
        Init("addTestDb3", out _, out PollingStationRepository repository);
        var entity = new PollingStationModel
        {
            Id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8"),
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new List<TagModel>
            {

            }
        };


        Task act() => repository.AddAsync(entity);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);

        exception.Message.Contains("At least 1 tag is required!");

    }

    [Fact]
    public async Task AddAsync_ShouldThrowExceptionNoTagNull()
    {
        // Arrange
        Init("addTestDb4", out _, out PollingStationRepository repository);
        var entity = new PollingStationModel
        {
            Id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8"),
            DisplayOrder = 1,
            Address = "123 Main St"
        };


        Task act() => repository.AddAsync(entity);

        //Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(act);

        exception.Message.Contains("At least 1 tag is required!");

    }

    [Fact]
    public async Task AddAsync_TagAlreadyExist()
    {
        // Arrange
        Init("addTestDb5", out AppDbContext context, out PollingStationRepository repository);
        var entity1 = new PollingStationModel
        {
            Id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8"),
            DisplayOrder = 1,
            Address = "123 Main St",
            Tags = new List<TagModel>
            {
                new TagModel {Key = "key1", Value = "value1"},
                new TagModel {Key = "key2", Value = "value2"}
            }
        };

        var entity2 = new PollingStationModel
        {
            Id = Guid.Parse("bffab6bc-ab8f-4197-a5fe-3c559dfb8d72"),
            DisplayOrder = 3,
            Address = "clslsl",
            Tags = new List<TagModel>
            {
                new TagModel {Key = "key1", Value = "value1"},
                new TagModel {Key = "key3", Value = "value2"}
            }
        };


        await repository.AddAsync(entity1);
        //Act


        var result = await repository.AddAsync(entity2);
        //Assert


        Assert.Equal(entity2, result);
        Assert.True(context.PollingStations.Count() == 2);
        Assert.True(context.Tags.Count() == 3);

    }
}

