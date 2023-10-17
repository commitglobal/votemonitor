using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories;
public class PollingStationRepositoryGetTests
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

    private void Init(string dbname, out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(dbname);
        var context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);
        repository.AddAsync(_pollingStations[0]).Wait();
        repository.AddAsync(_pollingStations[1]).Wait();
        repository.AddAsync(_pollingStations[2]).Wait();
        repository.AddAsync(_pollingStations[3]).Wait();
        repository.AddAsync(_pollingStations[4]).Wait();
    }
    public PollingStationRepositoryGetTests()
    {

    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPollingStation()
    {
        // Arrange
        Init("gettest1", out PollingStationRepository repository);

        var id = 1;

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        Assert.Equal(_pollingStations[0], result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException()
    {
        // Arrange
        Init("gettest2", out PollingStationRepository repository);

        var id = 6;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<PollingStationModel>>(() => repository.GetByIdAsync(id));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPollingStations()
    {

        // Arrange
        Init("gettest3", out PollingStationRepository repository);

        // Arrange

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(_pollingStations, result);
        Assert.True(result.Count() == 5, "Should be 5 PS in the repo");
    }
    [Fact]
    public async Task GetAllAsync_ShouldReturnPollingStationsByTags()
    {
        // Arrange
        Init("gettest4", out PollingStationRepository repository);

        var tags = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

        List<PollingStationModel> expectedResult = new List<PollingStationModel>()
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
                    Id = 4,
                    Address ="addr2",
                    DisplayOrder =1,
                    Tags = new List<TagModel>
                    {
                        new TagModel {Key = "key1", Value = "value1"},
                        new TagModel {Key = "key2", Value = "value2"},
                        new TagModel {Key = "ke3", Value = "value4"}
                    }
                }
        };

        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count() == 2, "Should be 2 PS in the repo");
        
        Assert.Equal(result[0], _pollingStations[0]);
        Assert.Equal(result[1], _pollingStations[3]);
    }

   



    [Fact]
    public async Task CountAsync_WithFilterCriteria_ShouldReturnFilteredPollingStationsCount()
    {
        // Arrange
        Init("gettest7", out PollingStationRepository repository);

        var filterCriteria = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };





        // Act
        var result = await repository.CountAsync(filterCriteria);

        // Assert
        Assert.True( result == 2 );

    }




}
