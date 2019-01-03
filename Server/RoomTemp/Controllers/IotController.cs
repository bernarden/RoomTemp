using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using RoomTemp.Data;
using RoomTemp.Models;

namespace RoomTemp.Controllers
{
    [Route("api/[controller]")]
    public class IotController : ControllerBase
    {
        private readonly TemperatureContext _temperatureContext;
        private readonly IMemoryCache _cache;

        public IotController(IMemoryCache cache, TemperatureContext temperatureContext)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

           var result = new DeviceDto
            {
                Id = device.Id,
                Name = device.Name,
            };
            return Ok(result);
        }

        [HttpGet("sensors")]
        public async Task<IActionResult> GetSensors()
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            var sensors = await GetCachedValue("AllSensors",
                async () => await _temperatureContext.Sensor.ToListAsync(),
                TimeSpan.FromHours(6));

            var result = sensors.Select(x => new SensorDto
            {
                Id = x.Id,
                Name = x.Name
            });
            return Ok(result);
        }

        [HttpPost("sensors")]
        public async Task<IActionResult> PostSensor([FromBody]string sensorName)
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingSensor = await _temperatureContext.Sensor
                .FirstOrDefaultAsync(x => x.Name.ToLower().Equals(sensorName.ToLower()));
            if (existingSensor == null)
            {
                var newSensor = new Sensor { Name = sensorName };
                var entityEntry = await _temperatureContext.Sensor.AddAsync(newSensor);
                await _temperatureContext.SaveChangesAsync();
                ResetCache("AllSensors");
                return Ok(new SensorDto
                {
                    Id = entityEntry.Entity.Id,
                    Name = entityEntry.Entity.Name
                });
            }

            return Ok(new SensorDto
            {
                Id = existingSensor.Id,
                Name = existingSensor.Name
            });
        }

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            var locations = await GetCachedValue("AllLocations",
                async () => await _temperatureContext.Location.ToListAsync(),
                TimeSpan.FromHours(6));

            var result = locations.Select(x => new LocationDto
            {
                Id = x.Id,
                Name = x.Name
            });
            return Ok(result);
        }

        [HttpPost("locations")]
        public async Task<IActionResult> PostLocation([FromBody]string locationName)
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingLocation = await _temperatureContext.Location
                .FirstOrDefaultAsync(x => x.Name.ToLower().Equals(locationName.ToLower()));
            if (existingLocation == null)
            {
                var newLocation = new Location { Name = locationName };
                var entityEntry = await _temperatureContext.Location.AddAsync(newLocation);
                await _temperatureContext.SaveChangesAsync();
                ResetCache("AllLocations");
                return Ok(new LocationDto
                {
                    Id = entityEntry.Entity.Id,
                    Name = entityEntry.Entity.Name
                });
            }

            return Ok(new LocationDto
            {
                Id = existingLocation.Id,
                Name = existingLocation.Name
            });
        }

        [HttpPost("readings")]
        public async Task<IActionResult> AddReading([FromBody] IEnumerable<TemperatureReadingDto> temperatureReadings)
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tempReadings = temperatureReadings.Select(x => new TempReading
            {
                DeviceId = device.Id,
                SensorId = x.SensorId,
                LocationId = x.LocationId,
                Temperature = x.Temperature,
                TakenAt = x.TakenAt
            });
            await _temperatureContext.TempReading.AddRangeAsync(tempReadings);
            await _temperatureContext.SaveChangesAsync();

            return Ok();
        }

        private async Task<Device> GetAuthorizedDevice()
        {
            Guid? apiKey = ExtractApiKey();
            if (apiKey == null)
            {
                return null;
            }

            var device = await GetCachedValue($"DeviceByApiKey.{apiKey.Value}",
                async () => await _temperatureContext.Device.Where(x => x.Key == apiKey).FirstOrDefaultAsync(),
                TimeSpan.FromHours(6));

            return device;
        }

        private async Task<T> GetCachedValue<T>(string key, Func<Task<T>> func, TimeSpan expireIn,
            Func<T, bool> shouldCacheResult = null)
        {
            if (_cache.TryGetValue(key, out T cacheEntry))
                return cacheEntry;

            var valueToCache = await func();
            if (shouldCacheResult?.Invoke(valueToCache) ?? true)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(expireIn);
                _cache.Set(key, valueToCache, cacheEntryOptions);
            }

            return valueToCache;
        }

        private void ResetCache(string key)
        {
            _cache.Remove(key);
        }

        private Guid? ExtractApiKey()
        {
            const string iotApiKeyParameterName = "IoTApiKey";
            if (!Request.Headers.ContainsKey(iotApiKeyParameterName))
            {
                return null;
            }

            StringValues values = HttpContext.Request.Headers[iotApiKeyParameterName];
            if (values.Count != 1)
            {
                return null;
            }

            if (Guid.TryParse(values.First(), out var result))
            {
                return result;
            }

            return null;
        }
    }
}