using System.Collections.Generic;

namespace RoomTemp.Models
{
    public class DeviceInfoWithSensorsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<SensorDto> AvailableSensors { get; set; }
        public IEnumerable<LocationDto> AvailableLocations { get; set; }
    }
}