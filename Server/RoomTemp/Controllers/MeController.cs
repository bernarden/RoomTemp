using System;
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

        public MeController(TemperatureContext temperatureContext)
        {
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
        }

        [HttpGet]
        public async Task<IActionResult> Me()
        {
            var device = await GetAuthorizedDevice(_temperatureContext);
            if (device == null)
            {
                return Unauthorized();
            }

            var sensors = await _temperatureContext.Sensor.ToListAsync();
            var result = new DeviceInfoWithSensorsDto
            {
                Id = device.DeviceId,
                Name = device.Name,
                AvailableSensors = sensors.Select(x => new SensorDto
                {
                    SensorId = x.SensorId,
                    Name = x.Name
                })
            };
            return Ok(result);
        }
    }
}