using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;
public class PollingStationRepositoryGetTests
{


    private readonly List<Domain.Models.PollingStation> _pollingStations = new()
            {
                new Domain.Models.PollingStation
                {
                    Id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8"),
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
                    Id =  Guid.Parse("bffab6bc-ab8f-4197-a5fe-3c559dfb8d72"),
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new Dictionary<string, string>()
                    {
                         {  "key1",  "value2"},
                         {  "key2",  "value3"},
                         {  "ke3",  "value3"}
                    }.ToTags()
                },
                 new Domain.Models.PollingStation
                {
                    Id = Guid.Parse("fed8db2e-c5a4-48e6-9eb1-58f899cd5f9f"),
                    Address ="addr3",
                    DisplayOrder =1,
                    Tags = new Dictionary<string, string>()
                    {
                         {  "key13",  "value1"},
                         {  "key2",  "value2"},
                         {  "ke3",  "value4"}
                    }.ToTags()
                },
                  new Domain.Models.PollingStation
                {
                    Id = Guid.Parse("8067d8fb-4270-473f-94e6-fdac927e3557"),
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new Dictionary<string, string>()
                    {
                         {  "key1",  "value1"},
                         {  "key2",  "value2"},
                         {  "ke3",  "value4"}
                    }.ToTags()
                },
                   new Domain.Models.PollingStation
                {
                    Id = Guid.Parse("afb6e443-9e37-4c9f-b737-f38352ffbfec"),
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new Dictionary<string, string>()
                    {
                         {  "key13",  "value3"},
                         {  "key2",  "value5"},
                         {  "ke3",  "value5"}
                    }.ToTags()
                }

            };

    private static void Init(out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);
    }


    private static void Init(out PollingStationRepository repository, List<Domain.Models.PollingStation> pollingStations)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        Init(out repository);
        foreach (var ps in pollingStations)
        {
            repository.AddAsync(ps).Wait();
        }

    }




    [Fact]
    public async Task GetByIdAsync_ShouldReturnPollingStation()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d52b8");

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        Assert.Equal(_pollingStations[0], result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var id = Guid.Parse("613066f1-1d8e-4119-bd58-d2dcb53d5878");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<Domain.Models.PollingStation>>(() => repository.GetByIdAsync(id));
    }



    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_WhenPageSizeIsLessThanZero()
    {
        // Arrange
        Init(out PollingStationRepository repository);

        // Act && Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetAllAsync(-1, 1));
    }

    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_WhenPageSizeIsGreaterThanZeroAndPageIsLessThanOneAndTagFilter()
    {
        // Arrange
        Init(out PollingStationRepository repository);
        var tags = new Dictionary<string, string>() { { "key1", "value1" } };
        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetAllAsync(tags, 1, 0));
    }

    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_FilterWhenPageSizeIsLessThanZeroAndTagFilter()
    {
        // Arrange
        Init(out PollingStationRepository repository);
        var tags = new Dictionary<string, string>() { { "key1", "value1" } };

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetAllAsync(-1, 1));
    }

    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_WhenPageSizeIsGreaterThanZeroAndPageIsLessThanOne()
    {
        // Arrange
        Init(out PollingStationRepository repository);
        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetAllAsync(1, 0));
    }
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPollingStations()
    {

        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        // Arrange

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(_pollingStations, result);
        Assert.True(result.Count() == 5, "Should be 5 PS in the repo");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPollingStationsNoFilterPagesizeeq6pageeq1()
    {

        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        // Arrange

        // Act
        var result = await repository.GetAllAsync(null, 6, 1);

        // Assert
        Assert.Equal(_pollingStations, result);
        Assert.True(result.Count() == 5, "Should be 5 PS in the repo");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturn1PollingStationsNoFilterPagesizeeq1pageeq1()
    {

        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        // Arrange

        // Act
        var result = (await repository.GetAllAsync(null, 1, 1)).ToList();

        // Assert
        Assert.Equal(_pollingStations[0], result[0]);
        Assert.True(result.Count == 1, "Should be 1 PS in the repo");
    }
    [Fact]
    public async Task GetAllAsync_ShouldReturnNoPollingStationsNoFilterPagesizeeq10pageeq2()
    {

        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        // Arrange

        // Act
        var result = (await repository.GetAllAsync(null, 10, 2)).ToList();

        // Assert

        Assert.True(result.Count == 0, "Should return NO PollingStation");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturn1PollingStationsNoFilterPagesizeeq1pageeq7()
    {

        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        // Arrange

        // Act
        var result = (await repository.GetAllAsync(null, 1, 7)).ToList();

        // Assert

        Assert.True(result.Count == 0, "Should be no PS returned");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPollingStationsByTagsFilterBy2TagsFoundIn2PS()
    {
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new Dictionary<string, string>()
        {
            {"key1", "value1" }   ,
            {"key2", "value2" }
        };




        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count == 2, "Should be 2 PS in the repo");

        Assert.Equal(result[0], _pollingStations[0]);
        Assert.Equal(result[1], _pollingStations[3]);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturn2PollingStations_FilterBy1TagsFoundIn2PS()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        Dictionary<string, string> tags = new()
        {
             {  "key1",  "value1" }
        };



        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count == 2, "Should be 2 PS in the repo");

        Assert.Equal(result[0], _pollingStations[0]);
        Assert.Equal(result[1], _pollingStations[3]);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturn2PollingStations_FilterBy1TagsFoundIn2PSPageSizeeq10Page1()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        Dictionary<string, string> tags = new() { { "key1", "value1" } };



        // Act
        var result = (await repository.GetAllAsync(tags, 10, 1)).ToList();

        // Assert
        Assert.True(result.Count == 2, "Should be 2 PS in the repo");

        Assert.Equal(result[0], _pollingStations[0]);
        Assert.Equal(result[1], _pollingStations[3]);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy1TagsFoundIn2PSPageSizeeq10Page2()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        Dictionary<string, string> tags = new() { { "key1", "value1" } };



        // Act
        var result = (await repository.GetAllAsync(tags, 10, 2)).ToList();

        // Assert
        Assert.True(result.Count == 0, "Should return 0 PS from the repo");


    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy1TagsFoundIn2PSPageSizeeq1Page2()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        Dictionary<string, string> tags = new() { { "key1", "value1" } };

        // Act
        var result = (await repository.GetAllAsync(tags, 1, 2)).ToList();

        // Assert
        Assert.True(result.Count == 1, "Should return 0 PS from the repo");
        Assert.Equal(result[0], _pollingStations[3]);

    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy1Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new Dictionary<string, string>()
            {
                    {  "key3321",  "value1" }
            };



        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count == 0, "Should be no PS in the PollingStation");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy4Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new Dictionary<string, string>()
            {
                {  "key1",  "value1" },
                {  "key2",  "value2" },
                {  "ke3",  "value3" },
                {  "ke13",  "value3" }
            };



        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count == 0, "Should be no PS in the PollingStation");
    }





    [Fact]
    public async Task CountAsync_ShouldReturn2_FilterBy2Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var filterCriteria = new Dictionary<string, string>()
            {
                {  "key1",  "value1" },
                {  "key2",  "value2" }
            };

        // Act
        var result = await repository.CountAsync(filterCriteria);

        // Assert
        Assert.True(result == 2);

    }

    [Fact]
    public async Task CountAsync_ShouldReturn2_FilterBy1Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var filterCriteria = new Dictionary<string, string>()
            {
                {  "key1",  "value1" },
                {  "key2",  "value2" }
            };

        // Act
        var result = await repository.CountAsync(filterCriteria);

        // Assert
        Assert.True(result == 2);

    }


    [Fact]
    public async Task CountAsync_ShouldReturn2_FilterBy4Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var filterCriteria = new Dictionary<string, string>()
            {
                {  "key1",  "value1" },
                {  "key2",  "value2" },
                {  "ke3",  "value3" },
                {  "ke13",  "value3" }
            };

        // Act
        var result = await repository.CountAsync(filterCriteria);

        // Assert
        Assert.True(result == 0);

    }

    [Fact]
    public async Task CountAsync_ShouldReturn5_NULLFilterCriteria()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);



        // Act
        var result = await repository.CountAsync(null);

        // Assert
        Assert.True(result == 5);

    }

    [Fact]
    public async Task CountAsync_ShouldReturn5_0FilterCriteria()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);



        // Act
        var result = await repository.CountAsync(null);

        // Assert
        Assert.True(result == 5);

    }
}
