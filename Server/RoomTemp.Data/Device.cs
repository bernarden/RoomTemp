using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Device
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
    }
}