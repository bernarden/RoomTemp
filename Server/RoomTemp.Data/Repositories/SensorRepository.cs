using System;
using System.Threading.Tasks;

namespace RoomTemp.Data.Repositories
{
    public class SensorRepository : BaseRepository<Sensor, int>
    {
        public SensorRepository(TemperatureContext temperatureContext) : base(temperatureContext)
        {
        }
    }
}