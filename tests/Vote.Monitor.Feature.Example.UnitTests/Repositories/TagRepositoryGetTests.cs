using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
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
                },
                 new PollingStationModel
                {
                    Id = 3,
                    Address ="addr3",
                    DisplayOrder =1,
                    Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key13", Value = "value1"},
                        new TagModel {Key = "key2", Value = "value2"},
                        new TagModel {Key = "ke3", Value = "value4"}
                    }
                },
                  new PollingStationModel
                {
                    Id = 4,
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key1", Value = "value1"},
                        new TagModel {Key = "key2", Value = "value2"},
                        new TagModel {Key = "ke3", Value = "value4"}
                    }
                },
                   new PollingStationModel
                {
                    Id = 5,
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key13", Value = "value3"},
                        new TagModel {Key = "key2", Value = "value5"},
                        new TagModel {Key = "ke3", Value = "value5"}
                    }
                }

            };

    private void Init(string dbname, out TagRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(dbname);
        var context = new AppDbContext(optionsBuilder.Options);
        PollingStationRepository pollingStationRepository = new PollingStationRepository(context);
        repository = new TagRepository(context);
        pollingStationRepository.AddAsync(_pollingStations[0]).Wait();
        pollingStationRepository.AddAsync(_pollingStations[1]).Wait();
        pollingStationRepository.AddAsync(_pollingStations[2]).Wait();
        pollingStationRepository.AddAsync(_pollingStations[3]).Wait();
        pollingStationRepository.AddAsync(_pollingStations[4]).Wait();
    }

    [Fact]
    public async Task GetAllTags_NoFilter_ShouldReturnAllTags()
    {
        // Arrange
        Init("gettest5", out TagRepository repository);

      


        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.True( result.Count() == 10, "should be 10 tags"  );
       //todo: Assert.True(result.Count() == 10, "Tags collection is not empty");
    }

}
