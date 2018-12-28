using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Location : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
    }
}