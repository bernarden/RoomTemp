using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RoomTemp.Data;
using RoomTemp.Domain;
using RoomTemp.Models;

namespace RoomTemp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebClientController : ControllerBase
    {
        private readonly ICachingService _cachingService;
        private readonly TemperatureContext _temperatureContext;

        public WebClientController(ICachingService cachingService, TemperatureContext temperatureContext)
        {
            _cachingService = cachingService ?? throw new ArgumentNullException(nameof(cachingService));
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
        }

        [HttpGet("tempReadings")]
        public IActionResult GetTempReadings(DateTimeOffset start, WebClientGetTempReadingRange range,
            int deviceId, int locationId, int sensorId)
        {
            if (start == DateTimeOffset.MinValue) return BadRequest("Please specify start.");
            if (range == WebClientGetTempReadingRange.Unspecified) return BadRequest("Please specify range.");
            if (deviceId <= 0) return BadRequest("Please specify valid device id.");
            if (locationId <= 0) return BadRequest("Please specify valid location id.");
            if (sensorId <= 0) return BadRequest("Please specify valid sensor id.");

            var (searchStartDateTime, searchEndDateTime) = GetSearchStartAndEndDates(start, range);

            var result = _cachingService.GetCachedValue(
                $"GetTempReadings.{searchStartDateTime:s}.{range:D}.{deviceId}.{locationId}.{sensorId}]",
                () =>
                {
                    var filteredTempReadings = _temperatureContext.TempReading
                        .Where(x => x.DeviceId == deviceId &&
                                    x.LocationId == locationId &&
                                    x.SensorId == sensorId &&
                                    x.TakenAt >= searchStartDateTime &&
                                    x.TakenAt < searchEndDateTime)
                        .AsEnumerable();

                    var temperatures = GetAverageTemperaturesGroupedByTime(filteredTempReadings, range);
                    return new WebClientTempReadingsDto
                    {
                        SearchStartDateTime = searchStartDateTime,
                        SearchEndDateTime = searchEndDateTime,
                        Temperatures = temperatures
                    };
                },
                r => searchEndDateTime <= DateTime.UtcNow.AddMinutes(-5)
                    ? TimeSpan.FromDays(7)
                    : TimeSpan.FromMinutes(5),
                r => true);
            return Ok(result);
        }

        private static List<WebClientTempReadingDto> GetAverageTemperaturesGroupedByTime(
            IEnumerable<TempReading> filteredTempReadings, WebClientGetTempReadingRange range)
        {
            // TODO: Currently EF Core doesn't evaluate AVERAGE function on server side (in DB) so have to do in memory. Revisit this later. 
            Func<TempReading, DateTime> takenAtGroupingFunction;
            switch (range)
            {
                case WebClientGetTempReadingRange.Hour:
                    // Grouping for every 10 seconds. Max: 360 readings.
                    takenAtGroupingFunction = s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day,
                        s.TakenAt.Hour, s.TakenAt.Minute, s.TakenAt.Second / 10 * 10);
                    break;
                case WebClientGetTempReadingRange.Day:
                    // Grouping for every 2 minutes. Max: 720 readings.
                    takenAtGroupingFunction = s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day,
                        s.TakenAt.Hour, s.TakenAt.Minute / 2 * 2, 0);
                    break;
                case WebClientGetTempReadingRange.Week:
                    // Grouping for every 15 minutes. Max: 672 readings.
                    takenAtGroupingFunction = s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day,
                        s.TakenAt.Hour, s.TakenAt.Minute / 15 * 15, 0);
                    break;
                case WebClientGetTempReadingRange.Month:
                    // Grouping for every 1 hour. Max: 672-744 readings.
                    takenAtGroupingFunction = s =>
                        new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day, s.TakenAt.Hour, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(range), range, null);
            }

            return filteredTempReadings
                .GroupBy(takenAtGroupingFunction, t => new { t.Temperature, t.TakenAt })
                .Select(g => new WebClientTempReadingDto
                {
                    TakenAt = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc),
                    Temperature = Math.Round(g.Average(a => a.Temperature), 2)
                }).ToList();
        }

        private (DateTime searchStartDateTime, DateTime searchEndDateTime) GetSearchStartAndEndDates(
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
                        .AddDays(start.DayOfWeek == DayOfWeek.Sunday ? -6 : -(int) start.DayOfWeek + 1)
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