using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Sensor
    {
        public int SensorId { get; set; }
        public string Name { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
    }
}