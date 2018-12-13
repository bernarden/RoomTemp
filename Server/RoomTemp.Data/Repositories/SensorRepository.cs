using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Sensor>> GetAll()
        {
            return await _temperatureContext.Sensor.ToListAsync();
        }

        public async Task<Sensor> CreateSensor(Sensor sensor)
        {
            var entityEntry = await _temperatureContext.AddAsync(sensor);
            await _temperatureContext.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public async Task<Sensor> UpdateSensor(Sensor sensor)
        {
            var entityEntry = _temperatureContext.Update(sensor);
            await _temperatureContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}