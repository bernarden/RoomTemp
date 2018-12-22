using GraphQL.Types;

namespace RoomTemp.Models.GraphQL.Sensor
{
    public class SensorInputType : InputObjectGraphType<Data.Sensor>
    {
        public SensorInputType()
        {
            Name = "SensorInput";
            Field(x => x.Name);
        }
    }
}