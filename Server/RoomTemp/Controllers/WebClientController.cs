using System;
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
        public async Task<IActionResult> GetTempReadings(DateTimeOffset start, WebClientGetTempReadingRange range, int deviceId, int locationId, int sensorId)
        {
            if (start == DateTimeOffset.MinValue) return BadRequest("Please specify start.");
            if (range == WebClientGetTempReadingRange.Unspecified) return BadRequest("Please specify range.");
            if (deviceId <= 0) return BadRequest("Please specify valid device id.");
            if (locationId <= 0) return BadRequest("Please specify valid location id.");
            if (sensorId <= 0) return BadRequest("Please specify valid sensor id.");

            // TODO: Deal with timezones on the start DateTimeOffset. Make sure to request UTC time from DB and return back the offset specified on request.
            
            // TODO: DB/Server grouping of the data to reduce the amount returned back.
            
            // TODO: Update caching key to include start and range.
            var result = await _cachingService.GetCachedValue($"GetTempReadings.{deviceId}.{locationId}.{sensorId}]",
                async () =>
                {
                    // TODO: Calculate the search range correctly.
                    var end = start.AddDays(1);
                    return await _temperatureContext.TempReading
                        .Where(x => x.DeviceId == deviceId &&
                                    x.LocationId == locationId &&
                                    x.SensorId == sensorId &&
                                    x.TakenAt >= start &&
                                    x.TakenAt <= end)
                        .Select(x => new WebClientTempReadingDto { TakenAt = x.TakenAt, Temperature = x.Temperature })
                        .ToListAsync();
                },
                TimeSpan.FromHours(6));
            return Ok(result);
        }
    }
}