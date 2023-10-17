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

    private void Init(string dbname,out AppDbContext context,  out PollingStationRepository repository)
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
}
