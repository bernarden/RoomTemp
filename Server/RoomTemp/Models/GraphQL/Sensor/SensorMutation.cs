using GraphQL.Types;
using RoomTemp.Data.Repositories;

namespace RoomTemp.Models.GraphQL.Sensor
{
    public class SensorMutation : ObjectGraphType
    {
        public SensorMutation(SensorRepository sensorRepository)
        {
            Name = "Mutation";

            FieldAsync<SensorType>(
                "createSensor",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<SensorInputType>> { Name = "sensor" }
                ),
                resolve: async context =>
                {
                    var sensor = context.GetArgument<Data.Sensor>("sensor");
                    sensor = sensorRepository.Add(sensor);
                    await sensorRepository.SaveChangesAsync();
                    return sensor;
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
                    var sensor = context.GetArgument<Data.Sensor>("sensor");
                    sensor.Id = context.GetArgument<int>("id");
                    sensor = sensorRepository.Update(sensor);
                    await sensorRepository.SaveChangesAsync();
                    return sensor;
                });
        }
    }
}