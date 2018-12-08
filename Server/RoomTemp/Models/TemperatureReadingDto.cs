using System;
using System.ComponentModel.DataAnnotations;

namespace RoomTemp.Models
{
    public class TemperatureReadingDto
    {
        [Range(-50, 100)]
        public decimal Temperature { get; set; }

        [Required]
        public DateTime TakenAt { get; set; }

        [Range(0, int.MaxValue)]
        public int LocationId { get; set; }

        [Range(0, int.MaxValue)]
        public int SensorId { get; set; }
    }
}