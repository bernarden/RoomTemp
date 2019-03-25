using System;
using System.Collections.Generic;

namespace RoomTemp.Models
{
    public class WebClientTempReadingsDto
    {
        public List<WebClientTempReadingDto> Temperatures { get; set; }
        public DateTime SearchStartDateTime { get; set; }
        public DateTime SearchEndDateTime { get; set; }
    }
}