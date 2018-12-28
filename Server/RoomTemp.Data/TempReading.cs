using System;

namespace RoomTemp.Data
{
    public class TempReading : IEntity<int>
    {
        public int Id { get; set; }

        public decimal Temperature { get; set; }
        public DateTime TakenAt { get; set; }

        public int SensorId { get; set; }
        public Sensor Sensor { get; set; }

        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}