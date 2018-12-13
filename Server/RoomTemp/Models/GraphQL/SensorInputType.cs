using GraphQL.Types;
using RoomTemp.Data;

namespace RoomTemp.Models.GraphQL
{
    public class SensorInputType : InputObjectGraphType<Sensor>
    {
        public SensorInputType()
        {
            Name = "SensorInput";
            Field(x => x.Name);
        }
    }
}