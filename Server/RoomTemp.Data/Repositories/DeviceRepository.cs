using System;
using System.Threading.Tasks;

namespace RoomTemp.Data.Repositories
{
    public class DeviceRepository : BaseRepository<Device, int>
    {
        public DeviceRepository(TemperatureContext temperatureContext) : base(temperatureContext)
        {
        }
    }
}