using System;
using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Device
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public Guid Key { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
    }
}