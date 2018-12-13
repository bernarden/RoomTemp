using GraphQL.Types;
using RoomTemp.Data;
using RoomTemp.Data.Repositories;

namespace RoomTemp.Models.GraphQL
{
    public class SensorMutation : ObjectGraphType
    {
        public SensorMutation(ISensorRepository sensorRepository)
        {
            Name = "Mutation";

            FieldAsync<SensorType>(
                "createSensor",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<SensorInputType>> { Name = "sensor" }
                ),
                resolve: async context =>
                {
                    var sensor = context.GetArgument<Sensor>("sensor");
                    return await sensorRepository.CreateSensor(sensor);
                });

            FieldAsync<SensorType>(
                "updateSensor",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                        { Name = "id", Description = "id of the sensor" },
                    new QueryArgument<NonNullGraphType<SensorInputType>> { Name = "sensor" }
                ),
                resolve: async context =>
                {
                    var sensor = context.GetArgument<Sensor>("sensor");
                    sensor.SensorId = context.GetArgument<int>("id");
                    return await sensorRepository.UpdateSensor(sensor);
                });
        }
    }
}