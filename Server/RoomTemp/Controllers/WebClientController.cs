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
                    // TODO: DB/Server grouping of the data to reduce the amount returned back.
                    return await _temperatureContext.TempReading
                        .Where(x => x.DeviceId == deviceId &&
                                    x.LocationId == locationId &&
                                    x.SensorId == sensorId &&
                                    x.TakenAt >= searchStartDateTime &&
                                    x.TakenAt < searchEndDateTime)
                        .Select(x => new WebClientTempReadingDto { TakenAt = x.TakenAt, Temperature = x.Temperature })
                        .ToListAsync();
                },
                TimeSpan.FromHours(12),
                (List<WebClientTempReadingDto> r) => searchEndDateTime <= DateTime.UtcNow.AddMinutes(-5));
            return Ok(result);
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