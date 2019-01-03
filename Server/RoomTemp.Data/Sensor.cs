using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoomTemp.Data
{
    public class Sensor : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<TempReading> TempReadings { get; set; }
    }
}