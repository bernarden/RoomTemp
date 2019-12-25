using System;
using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RoomTemp.Data;
using RoomTemp.Models;
using Xunit;

namespace RoomTemp.Tests
{
    public class WebClientControllerTest
    {
        public WebClientControllerTest()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("LocalTests")
                .UseStartup<Startup>()
            );
            _client = _server.CreateClient();
        }

        private readonly HttpClient _client;
        private readonly TestServer _server;

        [Fact(Skip = "Integration test")]
        [Trait("test", "integration")]
        public async void GetTempReadingsTest()
        {
            // Arrange
            var datetime = new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var temperatureContext = _server.Host.Services.GetService<TemperatureContext>();
            var device = temperatureContext.Device.Add(new Device { Name = "Test Device" });
            var location = temperatureContext.Location.Add(new Location { Name = "Test Location" });
            var sensor = temperatureContext.Sensor.Add(new Sensor { Name = "Test Sensor" });

            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddHours(-3).AddMinutes(5).AddSeconds(23),
                Temperature = 20, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddHours(-2).AddMinutes(15).AddSeconds(12),
                Temperature = 21, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddHours(-1).AddMinutes(10).AddSeconds(18),
                Temperature = 22, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddMinutes(19).AddSeconds(39),
                Temperature = 23, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddHours(1).AddMinutes(24).AddSeconds(1),
                Temperature = 24, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddHours(2).AddMinutes(31).AddSeconds(57),
                Temperature = 25, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            temperatureContext.TempReading.Add(new TempReading
            {
                TakenAt = datetime.AddHours(3).AddMinutes(23).AddSeconds(39),
                Temperature = 26, DeviceId = device.Entity.Id, SensorId = sensor.Entity.Id,
                LocationId = location.Entity.Id
            });
            await temperatureContext.SaveChangesAsync();

            var queryBuilder = new QueryBuilder
            {
                { "start", datetime.AddHours(1).ToString("s") + " +01:00" },
                { "range", WebClientGetTempReadingRange.Day.ToString("D") },
                { "deviceId", device.Entity.Id.ToString() },
                { "locationId", location.Entity.Id.ToString() },
                { "sensorId", sensor.Entity.Id.ToString() }
            };

            // Act
            var response = await _client.GetAsync("/api/WebClient/tempReadings" + queryBuilder.ToQueryString());

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            var result = JsonConvert.DeserializeObject<WebClientTempReadingsDto>(responseString);
            result.Should().NotBeNull();
            result.SearchStartDateTime.Should().Be(datetime.AddHours(-1));
            result.SearchEndDateTime.Should().Be(datetime.AddDays(1).AddHours(-1));
            result.Temperatures.Should().NotBeNullOrEmpty();
            result.Temperatures.Count().Should().Be(5);
        }
    }
}