using GraphQL.Types;
using RoomTemp.Data;

namespace RoomTemp.Models.GraphQL
{
    public class SensorType : ObjectGraphType<Sensor>
    {
        public SensorType()
        {
            Field(x => x.SensorId).Description("The Id of the Sensor.");
            Field(x => x.Name).Description("The name of the Sensor.");
        }
    }
}