//using Vote.Monitor.Core;
//using Vote.Monitor.Domain.Models;
//using Vote.Monitor.Feature.PollingStation.Repositories;
//using Xunit;

//namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;

//public class PollingStationRepositoryTagGetTests
//{
//    private readonly List<Domain.Models.PollingStation> _pollingStations = new()
//    {
//        new Domain.Models.PollingStation
//        {
//            Id = Guid.Parse("56021543-fc3b-447d-a7cd-a533448bb9e1"),
//            Address = "addr1",
//            DisplayOrder = 0,
//            Tags = new Dictionary<string, string>()
//            {
//                { "key1", "value1" }, { "key2", "value2" }, { "ke3", "value3" }
//            }.ToTags()
//        },
//        new Domain.Models.PollingStation
//        {
//            Id = Guid.Parse("7ea27240-0711-49de-98fb-1d3e9df2fffd"),
//            Address = "addr2",
//            DisplayOrder = 1,
//            Tags = new Dictionary<string, string>()
//            {
//                { "key1", "value2" }, { "key2", "value3" }, { "ke3", "value3" }
//            }.ToTags()
//        }
//    };



//    private static void Init(out PollingStationRepository repository, List<Domain.Models.PollingStation>? models = null)
//    {
//        Init(out repository);
//        if (models != null && models.Count > 0)
//            foreach (var st in models)
//            {
//                repository.AddAsync(st).Wait();
//            }
//    }
//    [Fact]
//    public async Task GetAllTags_NoFilter_ShouldReturnAllTags()
//    {
//        // Arrange
//        Init(out PollingStationRepository repository, _pollingStations);

//        // Act
//        var result = await repository.GetTagKeys();

//        // Assert
//        Assert.True(result.Count() == 3, "should be 3 tags keys");
//        //todo: Assert.True(result.Count() == 10, "Tags collection is not empty");
//    }

//    [Fact]
//    public async Task GetAllTags_NoFilterNoPollingStation_ShouldReturnNoTags()
//    {
//        // Arrange
//        Init(out PollingStationRepository repository);




//        // Act
//        var result = await repository.GetTagKeys();

//        // Assert
//        Assert.True(!result.Any(), "should be 0 tags value");
//        //todo: Assert.True(result.Count() == 10, "Tags collection is not empty");
//    }

//    [Fact]
//    public async Task GetTagsAsync_NoFilter_SelectTaginvalid_ReturnsNoTags()
//    {
//        // Arrange

//        Init(out PollingStationRepository repository, _pollingStations);


//        // Act
//        var result = await repository.GetTagValuesAsync("nokey", null);

//        // Assert
//        Assert.True(!result.Any(), "Should be no values");
//    }

//    [Fact]
//    public async Task GetTagsAsync_NoFilter_SelectTagValid_ReturnsNoTags()
//    {
//        // Arrange
//        Init(out PollingStationRepository repository, _pollingStations);
//        string selectKey = "key1";
//        var expectedTags = new List<TagModel>
//                {
//           new() { Key = "key1",  Value = "value1"},
//             new()    {Key = "key1",  Value ="value2"}
//        };


//        // Act
//        var result = (await repository.GetTagValuesAsync(selectKey, null)).ToList();

//        // Assert
//        Assert.True(result.Count == 2, "Should be 2 tags");
//        Assert.True(result[0].Key == selectKey);
//        Assert.True(result[0].Value == expectedTags[0].Value);
//        Assert.True(result[1].Key == selectKey);
//        Assert.True(result[1].Value == expectedTags[1].Value);
//    }

//    [Fact]
//    public async Task GetTagsAsync_Filter_SelectTaginvalid_ReturnsNoTags()
//    {
//        // Arrange

//        Init(out PollingStationRepository repository, _pollingStations);
//        var filter = new Dictionary<string, string>()
//            {
//           { "key1",  "value1"},

//        };

//        // Act
//        var result = await repository.GetTagValuesAsync("nokey", filter);

//        // Assert
//        Assert.True(!result.Any(), "Should be no values");
//    }

//    [Fact]
//    public async Task GetTagsAsync_Filter_SelectTagValid_Returns2Tags()
//    {
//        // Arrange

//        Init(out PollingStationRepository repository, _pollingStations);
//        var filter = new Dictionary<string, string>()
//            {
//            { "key1",  "value1"},

//        };
//        string selectTag = "key2";
//        var expectedTags = new List<TagModel>
//                {
//            new() {Key =  "key2",Value =  "value2"},
//                };
//        // Act
//        var result = (await repository.GetTagValuesAsync(selectTag, filter)).ToList();

//        // Assert
//        Assert.True(result.Count == 1, "Should be 1 tags");
//        Assert.True(result[0].Key == selectTag);
//        Assert.True(result[0].Value == expectedTags[0].Value);

//    }



//    [Fact]
//    public async Task GetTagsAsync_FilterNotExist_SelectTagValid_ReturnsNOTags()
//    {
//        // Arrange

//        Init(out PollingStationRepository repository, _pollingStations);
//        var filter = new Dictionary<string, string>()
//            {
//            { "key1",  "value1"},
//                 { "key3",  "value1"}

//        };
//        string selectTag = "key2";

//        // Act
//        var result = (await repository.GetTagValuesAsync(selectTag, filter)).ToList();

//        // Assert
//        Assert.True(!result.Any(), "Should be no values");

//    }
//}


