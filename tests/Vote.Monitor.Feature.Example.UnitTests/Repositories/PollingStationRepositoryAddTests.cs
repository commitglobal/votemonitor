using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Vote.Monitor.Core.Exceptions;
using Vote.Monitor.Domain.DataContext;
using Vote.Monitor.Domain.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;
using Xunit;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.Repositories
{
    public class PollingStationRepositoryAddTests
    {

        [Fact]
        public async Task AddAsync_ShouldAddPollingStation()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase("addTestDb1");
            var context = new AppDbContext(optionsBuilder.Options);
            var repository = new PollingStationRepository(context);

            var entity = new PollingStationModel
            {
                Id = 1,
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
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase("addTestDb2");
            var context = new AppDbContext(optionsBuilder.Options);
            var repository = new PollingStationRepository(context);
            var entity = new PollingStationModel
            {
                Id = 1,
                DisplayOrder = 1,
                Address = "123 Main St",
                Tags = new List<TagModel>
                {
                    new TagModel {Key = "key1", Value = "value1"},
                    new TagModel {Key = "key1", Value = "value2"}
                }
            };
            
            //act 
            Func<Task> act = () => repository.AddAsync(entity);

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(act);
        }

        [Fact]

        public async Task AddAsync_ShouldThrowExceptionNoTag()
        {
            // Arrange
            var repository = new PollingStationRepository(null);
            var entity = new PollingStationModel
            {
                Id = 1,
                DisplayOrder = 1,
                Address = "123 Main St",
                Tags = new List<TagModel>
                {

                }
            };


            Func<Task> act = () => repository.AddAsync(entity);

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(act);

            exception.Message.Contains("At least 1 tag is required!");

        }

        [Fact]
        public async Task AddAsync_ShouldThrowExceptionNoTagNull()
        {
            // Arrange
            var repository = new PollingStationRepository(null);
            var entity = new PollingStationModel
            {
                Id = 1,
                DisplayOrder = 1,
                Address = "123 Main St",
                Tags = null
            };


            Func<Task> act = () => repository.AddAsync(entity);

            //Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(act);

            exception.Message.Contains("At least 1 tag is required!");

        }

        [Fact]
        public async Task AddAsync_TagAlreadyExist()
        {
            // Arrange
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseInMemoryDatabase("addTestDb2");
            var context = new AppDbContext(optionsBuilder.Options);
            var repository = new PollingStationRepository(context);
            var entity1 = new PollingStationModel
            {
                Id = 1,
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
                Id = 2,
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
}
