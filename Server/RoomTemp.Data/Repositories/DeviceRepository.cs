using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RoomTemp.Data.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly TemperatureContext _temperatureContext;

        public DeviceRepository(TemperatureContext temperatureContext)
        {
            _temperatureContext = temperatureContext ?? throw new ArgumentNullException(nameof(temperatureContext));
        }

        public async Task<Device> Get(int id)
        {
            return await _temperatureContext.Device.FirstOrDefaultAsync(droid => droid.DeviceId == id);
        }

        public async Task<IEnumerable<Device>> GetAll()
        {
            return await _temperatureContext.Device.ToListAsync();
        }

        public async Task<Device> CreateDevice(Device device)
        {
            var entityEntry = await _temperatureContext.AddAsync(device);
            await _temperatureContext.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public async Task<Device> UpdateDevice(Device device)
        {
            var entityEntry = _temperatureContext.Update(device);
            await _temperatureContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}