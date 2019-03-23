using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetTempReadings(DateTimeOffset start, WebClientGetTempReadingRange range,
            int deviceId, int locationId, int sensorId)
        {
            if (start == DateTimeOffset.MinValue) return BadRequest("Please specify start.");
            if (range == WebClientGetTempReadingRange.Unspecified) return BadRequest("Please specify range.");
            if (deviceId <= 0) return BadRequest("Please specify valid device id.");
            if (locationId <= 0) return BadRequest("Please specify valid location id.");
            if (sensorId <= 0) return BadRequest("Please specify valid sensor id.");

            var (searchStartDateTime, searchEndDateTime) = GetSearchStartAndEndDates(start, range);

            List<WebClientTempReadingDto> result = await _cachingService.GetCachedValue(
                $"GetTempReadings.{searchStartDateTime:s}.{range:D}.{deviceId}.{locationId}.{sensorId}]",
                async () =>
                {
                    IQueryable<TempReading> filteredTempReadings = _temperatureContext.TempReading
                        .Where(x => x.DeviceId == deviceId &&
                                    x.LocationId == locationId &&
                                    x.SensorId == sensorId &&
                                    x.TakenAt >= searchStartDateTime &&
                                    x.TakenAt < searchEndDateTime);
                    IQueryable<WebClientTempReadingDto> groupedTempReadings = ApplyGrouping(filteredTempReadings, range);
                    return await groupedTempReadings.ToListAsync();
                },
                TimeSpan.FromDays(7),
                (List<WebClientTempReadingDto> r) => searchEndDateTime <= DateTime.UtcNow.AddMinutes(-5));
            return Ok(result);
        }

        private static IQueryable<WebClientTempReadingDto> ApplyGrouping(IQueryable<TempReading> filteredTempReadings, WebClientGetTempReadingRange range)
        {
            switch (range)
            {
                case WebClientGetTempReadingRange.Hour:
                    // Grouping for every 10 seconds. Max: 360 readings.
                    return filteredTempReadings
                        .GroupBy(s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day, s.TakenAt.Hour, s.TakenAt.Minute, s.TakenAt.Second / 10 * 10), t => new { t.Temperature, t.TakenAt })
                        .Select(g => new WebClientTempReadingDto { TakenAt = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc), Temperature = Math.Round(g.Average(a => a.Temperature), 2) });
                case WebClientGetTempReadingRange.Day:
                    // Grouping for every 2 minutes. Max: 720 readings.
                    return filteredTempReadings
                        .GroupBy(s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day, s.TakenAt.Hour, s.TakenAt.Minute / 2 * 2, 0), t => new { t.Temperature, t.TakenAt })
                        .Select(g => new WebClientTempReadingDto{ TakenAt = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc), Temperature = Math.Round(g.Average(a => a.Temperature), 2) });
                case WebClientGetTempReadingRange.Week:
                    // Grouping for every 15 minutes. Max: 672 readings.
                    return filteredTempReadings
                        .GroupBy(s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day, s.TakenAt.Hour, s.TakenAt.Minute / 15 * 15, 0), t => new { t.Temperature, t.TakenAt })
                        .Select(g => new WebClientTempReadingDto { TakenAt = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc), Temperature = Math.Round(g.Average(a => a.Temperature), 2) });
                case WebClientGetTempReadingRange.Month:
                    // Grouping for every 1 hour. Max: 672-744 readings.
                    return filteredTempReadings
                        .GroupBy(s => new DateTime(s.TakenAt.Year, s.TakenAt.Month, s.TakenAt.Day, s.TakenAt.Hour, 0, 0), t => new { t.Temperature, t.TakenAt })
                        .Select(g => new WebClientTempReadingDto { TakenAt = DateTime.SpecifyKind(g.Key, DateTimeKind.Utc), Temperature = Math.Round(g.Average(a => a.Temperature), 2) });
                default:
                    throw new ArgumentOutOfRangeException(nameof(range), range, null);
            }
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