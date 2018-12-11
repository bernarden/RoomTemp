using GraphQL.Types;
using RoomTemp.Data;

namespace RoomTemp.Models.GraphQL
{
    public class SensorQuery : ObjectGraphType
    {
        public SensorQuery()
        {
            Field<SensorType>(
                "hero",
                resolve: context => new Sensor(){SensorId = 12, Name = "name"}
            );
        }
    }
}