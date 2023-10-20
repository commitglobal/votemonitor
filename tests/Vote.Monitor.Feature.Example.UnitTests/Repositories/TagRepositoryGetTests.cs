using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.GetPollingStationsTagValues;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;

public class TagRepositoryGetTests
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



    private void Init( out TagRepository repository, out PollingStationRepository pollingStationRepository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var context = new AppDbContext(optionsBuilder.Options);
        pollingStationRepository = new PollingStationRepository(context);
        repository = new TagRepository(context);
       
    }


    private void Init( out TagRepository repository, List<PollingStationModel>? models=null)
    {
        Init( out  repository, out PollingStationRepository pollingStationRepository);
        if (models != null && models.Count > 0)
            foreach(var st in models)
            {
                pollingStationRepository.AddAsync(st).Wait();
            }
    }
    [Fact]
    public async Task GetAllTags_NoFilter_ShouldReturnAllTags()
    {
        // Arrange
        Init( out TagRepository repository,  _pollingStations);

      


        // Act
        var result = await repository.GetAllTagKeysAsync();

        // Assert
        Assert.True( result.Count() == 3, "should be 3 tags keys"  );
       //todo: Assert.True(result.Count() == 10, "Tags collection is not empty");
    }

    [Fact]
    public async Task GetAllTags_NoFilterNoPollingStation_ShouldReturnNoTags()
    {
        // Arrange
        Init(out TagRepository repository);




        // Act
        var result = await repository.GetAllTagKeysAsync();

        // Assert
        Assert.True(result.Count() == 0, "should be 0 tags value");
        //todo: Assert.True(result.Count() == 10, "Tags collection is not empty");
    }

    [Fact]
    public async Task GetTagsAsync_NoFilter_SelectTaginvalid_ReturnsNoTags()
    {
        // Arrange
        
        Init(out TagRepository repository,  _pollingStations );


        // Act
        var result = await repository.GetTagsAsync("nokey",null);

        // Assert
        Assert.True(result.Count()== 0, "Should be no values");
    }

    [Fact]
    public async Task GetTagsAsync_NoFilter_SelectTagValid_ReturnsNoTags()
    {
        // Arrange
        Init(out TagRepository repository, _pollingStations);
        string selectKey = "key1";
        var expectedTags =new List<TagModel>()
            {
                new TagModel(){Key = "key1", Value = "value1"},
                new TagModel(){Key = "key1", Value = "value2"}
                };


        // Act
        var result = (await repository.GetTagsAsync(selectKey, null)).ToList();

        // Assert
        Assert.True(result.Count() == 2, "Should be 2 tags");
        Assert.True(result[0].Key == selectKey);
        Assert.True(result[0].Value == expectedTags[0].Value);
        Assert.True(result[1].Key == selectKey);
        Assert.True(result[1].Value == expectedTags[1].Value);
    }

    [Fact]
    public async Task GetTagsAsync_Filter_SelectTaginvalid_ReturnsNoTags()
    {
        // Arrange

        Init(out TagRepository repository,   _pollingStations);
        List<TagModel> filter = new List<TagModel>()
        {
                new TagModel(){Key = "key1", Value = "value1"},
                
        };  

        // Act
        var result = await repository.GetTagsAsync("nokey", filter);

        // Assert
        Assert.True(result.Count() == 0, "Should be no values");
    }

    [Fact]
    public async Task GetTagsAsync_Filter_SelectTagValid_Returns2Tags()
    {
        // Arrange

        Init(out TagRepository repository, _pollingStations);
        List<TagModel> filter = new List<TagModel>()
        {
                new TagModel(){Key = "key1", Value = "value1"},

        };
        string selectTag = "key2";
        var expectedTags = new List<TagModel>()
            {
                 new TagModel {Key = "key2", Value = "value2"},
                };
        // Act
        var result = (await repository.GetTagsAsync(selectTag, filter)).ToList();

        // Assert
        Assert.True(result.Count() == 1, "Should be 1 tags");
        Assert.True(result[0].Key == selectTag);
        Assert.True(result[0].Value == expectedTags[0].Value);

    }



    [Fact]
    public async Task GetTagsAsync_FilterNotExist_SelectTagValid_ReturnsNOTags()
    {
        // Arrange

        Init(out TagRepository repository, _pollingStations);
        List<TagModel> filter = new List<TagModel>()
        {
                new TagModel(){Key = "key1", Value = "value1"},
                 new TagModel(){Key = "key3", Value = "value1"}

        };
        string selectTag = "key2";
    
        // Act
        var result = (await repository.GetTagsAsync(selectTag, filter)).ToList();

        // Assert
        Assert.True(result.Count() == 0, "Should be no values");

    }
}


