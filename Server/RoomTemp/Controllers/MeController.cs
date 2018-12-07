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