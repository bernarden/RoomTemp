using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomTemp.Data;
using RoomTemp.Models;

namespace RoomTemp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeController : IotControllerBase
    {
        private readonly TemperatureContext _temperatureContext;

        public MeController(IServiceProvider serviceProvider, TemperatureContext temperatureContext)
            : base(serviceProvider)
        {
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
        }

        [HttpGet]
        public async Task<IActionResult> Me()
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            var sensors = await GetCachedValue("AllSensors",
                async () => await _temperatureContext.Sensor.ToListAsync(),
                TimeSpan.FromHours(6));

            var result = new DeviceInfoWithSensorsDto
            {
                Id = device.Id,
                Name = device.Name,
                AvailableSensors = sensors.Select(x => new SensorDto
                {
                    SensorId = x.Id,
                    Name = x.Name
                })
            };
            return Ok(result);
        }

        [HttpPost("/AddReadings")]
        public async Task<IActionResult> AddReading([FromBody] IEnumerable<TemperatureReadingDto> temperatureReadings)
        {
            var device = await GetAuthorizedDevice();
            if (device == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(temperatureReadings);
            }

            var tempReadings = temperatureReadings.Select(x => new TempReading
            {
                DeviceId = device.Id,
                SensorId = x.SensorId,
                Temperature = x.Temperature,
                TakenAt = x.TakenAt
            });
            await _temperatureContext.TempReading.AddRangeAsync(tempReadings);
            await _temperatureContext.SaveChangesAsync();

            return Ok();
        }
    }
}