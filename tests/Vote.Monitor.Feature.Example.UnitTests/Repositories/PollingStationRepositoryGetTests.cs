using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
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

    private void Init(out PollingStationRepository repository)
    {
        //,out  DbContextOptionsBuilder<AppDbContext> optionsBuilder ,out AppDbContext context
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        var context = new AppDbContext(optionsBuilder.Options);
        repository = new PollingStationRepository(context);
    }


    private void Init(out PollingStationRepository repository, List<PollingStationModel> pollingStations)
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
        Init(out PollingStationRepository repository, _pollingStations);

        var id = 6;

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException<PollingStationModel>>(() => repository.GetByIdAsync(id));
    }



    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_WhenPageSizeIsLessThanZero()
    {
        // Arrange
        Init(out PollingStationRepository repository);

        // Act
         Func<Task> act = () => repository.GetAllAsync(-1, 1);
        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetAllAsync(-1, 1));
    }

    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_WhenPageSizeIsGreaterThanZeroAndPageIsLessThanOneAndTagFilter()
    {
        // Arrange
        Init(out PollingStationRepository repository);
        var tags = new List<TagModel>() { new TagModel { Key = "key1", Value = "value1" } };
        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetAllAsync(tags,1, 0));
    }

    [Fact]
    public async Task GetAllAsync_ThrowsArgumentOutOfRangeException_FilterWhenPageSizeIsLessThanZeroAndTagFilter()
    {
        // Arrange
        Init(out PollingStationRepository repository);
        var tags = new List<TagModel>() { new TagModel { Key = "key1", Value = "value1" } };

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
        Assert.True(result.Count() == 1, "Should be 1 PS in the repo");
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

        Assert.True(result.Count() == 0, "Should return NO PollingStation");
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

        Assert.True(result.Count() == 0, "Should be no PS returned");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPollingStationsByTagsFilterBy2TagsFoundIn2PS()
    {
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>
        {
            new TagModel{Key="key1", Value="value1" }   ,
            new TagModel{Key="key2", Value="value2" }
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
    public async Task GetAllAsync_ShouldReturn2PollingStations_FilterBy1TagsFoundIn2PS()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>();
        tags.Add(new TagModel{ Key = "key1", Value = "value1" });
            
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
    public async Task GetAllAsync_ShouldReturn2PollingStations_FilterBy1TagsFoundIn2PSPageSizeeq10Page1()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>();
        tags.Add(new TagModel { Key = "key1", Value = "value1" });

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
        var result = (await repository.GetAllAsync(tags,10,1)).ToList();

        // Assert
        Assert.True(result.Count() == 2, "Should be 2 PS in the repo");

        Assert.Equal(result[0], _pollingStations[0]);
        Assert.Equal(result[1], _pollingStations[3]);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy1TagsFoundIn2PSPageSizeeq10Page2()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>();
        tags.Add(new TagModel { Key = "key1", Value = "value1" });

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
        var result = (await repository.GetAllAsync(tags, 10, 2)).ToList();

        // Assert
        Assert.True(result.Count() == 0, "Should return 0 PS from the repo");

       
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy1TagsFoundIn2PSPageSizeeq1Page2()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>();
        tags.Add(new TagModel { Key = "key1", Value = "value1" });

        List<PollingStationModel> expectedResult = new List<PollingStationModel>()
        {

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
        var result = (await repository.GetAllAsync(tags, 1, 2)).ToList();

        // Assert
        Assert.True(result.Count() == 1, "Should return 0 PS from the repo");
        Assert.Equal(result[0], _pollingStations[3]);

    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy1Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>
            {
                    new TagModel{ Key = "key3321", Value = "value1" }
            };



        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count() == 0, "Should be no PS in the PollingStation");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnNOPollingStations_FilterBy4Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var tags = new List<TagModel>
            {
                new TagModel{ Key = "key1", Value = "value1" },
                new TagModel{ Key = "key2", Value = "value2" },
                new TagModel{ Key = "ke3", Value = "value3" },
                new TagModel{ Key = "ke13", Value = "value3" }
            };



        // Act
        var result = (await repository.GetAllAsync(tags)).ToList();

        // Assert
        Assert.True(result.Count() == 0, "Should be no PS in the PollingStation");
    }





    [Fact]
    public async Task CountAsync_ShouldReturn2_FilterBy2Tags()
    {
        // Arrange
        Init(out PollingStationRepository repository, _pollingStations);

        var filterCriteria = new List<TagModel>
            {
                new TagModel{ Key = "key1", Value = "value1" },
                new TagModel{ Key = "key2", Value = "value2" }
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

        var filterCriteria = new List<TagModel>
            {
                new TagModel{ Key = "key1", Value = "value1" },
                new TagModel{ Key = "key2", Value = "value2" }
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

        var filterCriteria = new List<TagModel>
            {
                new TagModel{ Key = "key1", Value = "value1" },
                new TagModel{ Key = "key2", Value = "value2" },
                new TagModel{ Key = "ke3", Value = "value3" },
                new TagModel{ Key = "ke13", Value = "value3" }
            };

        // Act
        var result = await repository.CountAsync(filterCriteria);

        // Assert
        Assert.True(result==0);

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
        var result = await repository.CountAsync(new List<TagModel>());

        // Assert
        Assert.True(result == 5);

    }



}
