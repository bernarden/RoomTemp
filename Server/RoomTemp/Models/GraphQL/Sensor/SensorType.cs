using GraphQL.Types;

namespace RoomTemp.Models.GraphQL.Sensor
{
    public class SensorType : ObjectGraphType<Data.Sensor>
    {
        public SensorType()
        {
            Field(x => x.SensorId).Description("The Id of the Sensor.");
            Field(x => x.Name).Description("The name of the Sensor.");
        }
    }
}