using System;
using System.Collections.Generic;

namespace RoomTemp.Data
{
    public class Device : IEntity<int>
    {
        public string Name { get; set; }
        public Guid Key { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
        public int Id { get; set; }
    }
}