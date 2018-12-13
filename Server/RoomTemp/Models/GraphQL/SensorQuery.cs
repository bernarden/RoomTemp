﻿using GraphQL.Types;
using RoomTemp.Data.Repositories;

namespace RoomTemp.Models.GraphQL
{
    public class SensorQuery : ObjectGraphType
    {
        public SensorQuery(ISensorRepository sensorRepository)
        {
            FieldAsync<SensorType>(
                "sensor",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the sensor" }
                ),
                resolve: async context => await sensorRepository.Get(context.GetArgument<int>("id"))
            );

            FieldAsync<SensorType>(
                "sensors",
                resolve: async context => await sensorRepository.GetAll()
            );
        }
    }
}