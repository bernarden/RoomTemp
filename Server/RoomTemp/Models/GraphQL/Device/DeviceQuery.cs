using GraphQL.Types;
using RoomTemp.Data.Repositories;
using RoomTemp.Models.GraphQL.Device;

namespace RoomTemp.Models.GraphQL.Device
{
    public class DeviceQuery : ObjectGraphType
    {
        public DeviceQuery(DeviceRepository deviceRepository)
        {
            FieldAsync<DeviceType>(
                "device",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the device" }
                ),
                resolve: async context => await deviceRepository.Get(context.GetArgument<int>("id"))
            );

            FieldAsync<DeviceType>(
                "devices",
                resolve: async context => await deviceRepository.GetAll()
            );
        }
    }
}