using GraphQL.Types;
using RoomTemp.Data.Repositories;

namespace RoomTemp.Models.GraphQL
{
    public class SensorQuery : ObjectGraphType
    {
        public SensorQuery(ISensorRepository sensorRepository)
        {
            Field<SensorType>(
                "hero",
                resolve: context => sensorRepository.Get(1)
            );
        }
    }
}