using GraphQL.Types;
using RoomTemp.Models.GraphQL.Device;
using RoomTemp.Models.GraphQL.Sensor;

namespace RoomTemp.Models.GraphQL
{
    public class Query : ObjectGraphType
    {
        public Query()
        {
            Name = "Query";
            Field<SensorType>("sensor",  resolve: context => new {});
            Field<DeviceType>("device",  resolve: context => new {});
        }
    }
}