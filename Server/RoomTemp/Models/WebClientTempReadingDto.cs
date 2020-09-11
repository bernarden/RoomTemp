using System;

namespace RoomTemp.Models
{
    public class WebClientTempReadingDto
    {
        private DateTime _takenAt;
        private decimal _temperature;

        public decimal Temperature
        {
            get => _temperature;
            set => _temperature = Math.Round(value, 2);
        }

        public DateTime TakenAt
        {
            get => _takenAt;
            set => _takenAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}