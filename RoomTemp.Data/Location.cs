using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Location
    {
        public int LocationId { get; set; }
        public string Name { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
    }
}