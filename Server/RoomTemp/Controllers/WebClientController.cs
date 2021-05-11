using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoomTemp.Data;
using RoomTemp.Domain;
using RoomTemp.Models;

namespace RoomTemp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebClientController : ControllerBase
    {
        private readonly TemperatureContext _temperatureContext;
        private readonly ICachingService _cachingService;

        public WebClientController(TemperatureContext temperatureContext, ICachingService cachingService)
        {
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
            _cachingService = cachingService ?? throw new ArgumentNullException(nameof(cachingService));
        }

        [HttpGet("tempReadings")]
        public async Task<IActionResult> GetTempReadings(DateTimeOffset start, WebClientGetTempReadingRange range,
            int deviceId, int locationId, int sensorId)
        {
            if (start == DateTimeOffset.MinValue) return BadRequest("Please specify start.");
            if (range == WebClientGetTempReadingRange.Unspecified) return BadRequest("Please specify range.");
            if (deviceId <= 0) return BadRequest("Please specify valid device id.");
            if (locationId <= 0) return BadRequest("Please specify valid location id.");
            if (sensorId <= 0) return BadRequest("Please specify valid sensor id.");

            var (searchStartDateTime, searchEndDateTime) = GetSearchStartAndEndDates(start, range);
            var result = await _cachingService.GetCachedValueAsync(
                $"GetTempReadings.{searchStartDateTime:s}.{range:D}.{deviceId}.{locationId}.{sensorId}]",
                async () =>
                {
                    if (_temperatureContext.Database.IsSqlServer())
                    {
                        var database = Startup.Configuration.GetConnectionString("database");
                        var connectionString = Startup.Configuration.GetConnectionString(database);

                        await using SqlConnection connection = new(connectionString);
                        await connection.OpenAsync();
                        var temperatureReadings = await connection.QueryAsync<WebClientTempReadingDto>(
                            "sp_GetAggregatedTemperatureReadings",
                            new { deviceId, locationId, sensorId, searchStartDateTime, searchEndDateTime },
                            commandType: CommandType.StoredProcedure);
                        return temperatureReadings;
                    }

                    var temperatures = (await _temperatureContext.TempReading
                            .Where(x => x.DeviceId == deviceId &&
                                        x.LocationId == locationId &&
                                        x.SensorId == sensorId &&
                                        x.TakenAt >= searchStartDateTime &&
                                        x.TakenAt < searchEndDateTime)
                            .ToListAsync())
                        .GroupBy(s =>
                                new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day, s.TakenAt.Hour, 0, 0),
                            t => new { t.Temperature, t.TakenAt })
                        .Select(g => new WebClientTempReadingDto
                        {
                            TakenAt = g.Key,
                            Temperature = g.Average(a => a.Temperature)
                        }).ToList();

                    return temperatures;
                },
                _ => TimeSpan.FromTicks(1),
                _ => true);
            return Ok(new WebClientTempReadingsDto
            {
                Temperatures = result,
                SearchStartDateTime = searchStartDateTime,
                SearchEndDateTime = searchEndDateTime,
            });
        }

        private static (DateTime searchStartDateTime, DateTime searchEndDateTime) GetSearchStartAndEndDates(
            DateTimeOffset start, WebClientGetTempReadingRange range)
        {
            DateTime searchStartDateTime;
            DateTime searchEndDateTime;
            switch (range)
            {
                case WebClientGetTempReadingRange.Hour:
                    searchStartDateTime = start
                        .AddMinutes(-start.Minute)
                        .AddSeconds(-start.Second)
                        .AddMilliseconds(-start.Millisecond)
                        .UtcDateTime;
                    searchEndDateTime = searchStartDateTime.AddHours(1);
                    break;
                case WebClientGetTempReadingRange.Day:
                    searchStartDateTime = start
                        .AddHours(-start.Hour)
                        .AddMinutes(-start.Minute)
                        .AddSeconds(-start.Second)
                        .AddMilliseconds(-start.Millisecond)
                        .UtcDateTime;
                    searchEndDateTime = searchStartDateTime.AddDays(1);
                    break;
                case WebClientGetTempReadingRange.Week:
                    searchStartDateTime = start
                        .AddDays(start.DayOfWeek == DayOfWeek.Sunday ? -6 : -(int)start.DayOfWeek + 1)
                        .AddHours(-start.Hour)
                        .AddMinutes(-start.Minute)
                        .AddSeconds(-start.Second)
                        .AddMilliseconds(-start.Millisecond)
                        .UtcDateTime;
                    searchEndDateTime = searchStartDateTime.AddDays(7);
                    break;
                case WebClientGetTempReadingRange.Month:
                    searchStartDateTime = start
                        .AddDays(-start.Day + 1)
                        .AddHours(-start.Hour)
                        .AddMinutes(-start.Minute)
                        .AddSeconds(-start.Second)
                        .AddMilliseconds(-start.Millisecond)
                        .UtcDateTime;
                    searchEndDateTime = searchStartDateTime.AddMonths(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(range), range, null);
            }

            return (searchStartDateTime, searchEndDateTime);
        }
    }
}