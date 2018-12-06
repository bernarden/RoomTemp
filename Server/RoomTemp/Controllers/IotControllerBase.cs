using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using RoomTemp.Data;

namespace RoomTemp.Controllers
{
    public class IotControllerBase : ControllerBase
    {
        protected async Task<Device> GetAuthorizedDevice(TemperatureContext temperatureContext)
        {
            Guid? apiKey = ExtractApiKey();
            if (apiKey == null)
            {
                return null;
            }

            var device = await temperatureContext.Device.Where(x => x.Key == apiKey).FirstOrDefaultAsync();
            return device;
        }

        private Guid? ExtractApiKey()
        {
            const string iotApiKeyParameterName = "IoTApiKey";
            if (!Request.Headers.ContainsKey(iotApiKeyParameterName))
            {
                return null;
            }

            StringValues values = Request.Headers[iotApiKeyParameterName];
            if (values.Count != 1)
            {
                return null;
            }

            if (Guid.TryParse(values.First(), out var result))
            {
                return result;
            }

            return null;
        }
    }
}