using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RoomTemp.Data;
using RoomTemp.Data.Repositories;
using Xunit;

namespace RoomTemp.Tests
{
    public class SensorRepositoryTest
    {
        private static TemperatureContext CreateTemperatureContext()
        {
            var options = new DbContextOptionsBuilder<TemperatureContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var temperatureContext = new TemperatureContext(options);
            return temperatureContext;
        }

        [Fact]
        public async Task ShouldCreateEntity()
        {
            // Arrange
            var temperatureContext = CreateTemperatureContext();
            var repository = new SensorRepository(temperatureContext);

            // Act
            var createdEntity = repository.Add(new Sensor { Name = "Name" });
            var saved = await repository.SaveChangesAsync();

            // Assert
            saved.Should().BeTrue();
            createdEntity.Id.Should().BeGreaterThan(0);
            createdEntity.Name.Should().Be("Name");
            createdEntity.TempReadings.Should().BeNull();
        }

        [Fact]
        public async Task ShouldUpdateEntity()
        {
            // Arrange
            var temperatureContext = CreateTemperatureContext();
            var repository = new SensorRepository(temperatureContext);
            var entity = repository.Add(new Sensor { Name = "Name" });
            await repository.SaveChangesAsync();
            entity.Name = "New name";

            // Act
            var updatedEntity = repository.Update(entity);
            var saved = await repository.SaveChangesAsync();

            // Assert
            saved.Should().BeTrue();
            updatedEntity.Id.Should().Be(entity.Id);
            updatedEntity.Name.Should().Be("New name");
            updatedEntity.TempReadings.Should().BeEquivalentTo(entity.TempReadings);
        }
        
        [Fact]
        public async Task ShouldGetEntity()
        {
            // Arrange
            var temperatureContext = CreateTemperatureContext();
            var repository = new SensorRepository(temperatureContext);
            var entity = repository.Add(new Sensor { Name = "Name" });
            await repository.SaveChangesAsync();

            // Act
            var loadedEntity = await repository.Get(entity.Id);

            // Assert
            loadedEntity.Id.Should().Be(entity.Id);
            loadedEntity.Name.Should().Be(entity.Name);
            loadedEntity.TempReadings.Should().BeEquivalentTo(entity.TempReadings);
        }
        
        [Fact]
        public async Task ShouldGetAllEntity()
        {
            // Arrange
            var temperatureContext = CreateTemperatureContext();
            var repository = new SensorRepository(temperatureContext);
            var entity = repository.Add(new Sensor { Name = "Name" });
            await repository.SaveChangesAsync();

            // Act
            var loadedEntities = await repository.GetAll();

            // Assert
            loadedEntities.Count.Should().Be(1);
            loadedEntities.First().Id.Should().Be(entity.Id);
            loadedEntities.First().Name.Should().Be(entity.Name);
            loadedEntities.First().TempReadings.Should().BeEquivalentTo(entity.TempReadings);
        }
    }
}