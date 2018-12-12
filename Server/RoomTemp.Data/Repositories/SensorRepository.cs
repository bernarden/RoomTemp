using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RoomTemp.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly TemperatureContext _temperatureContext;

        public SensorRepository(TemperatureContext temperatureContext)
        {
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
        }

        public async Task<Sensor> Get(int id)
        {
            return await _temperatureContext.Sensor.FirstOrDefaultAsync(droid => droid.SensorId == id);
        }
    }
}