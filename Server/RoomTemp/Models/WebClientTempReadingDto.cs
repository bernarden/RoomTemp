using System;

namespace RoomTemp.Models
{
    public class WebClientTempReadingDto
    {
        public decimal Temperature { get; set; }
        public DateTime TakenAt { get; set; }
    }
}