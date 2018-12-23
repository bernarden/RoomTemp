using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Sensor : IEntity<int>
    {
        public string Name { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
        public int Id { get; set; }
    }
}