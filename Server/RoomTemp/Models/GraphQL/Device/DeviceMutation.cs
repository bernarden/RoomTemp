using GraphQL.Types;
using RoomTemp.Data.Repositories;
using RoomTemp.Models.GraphQL.Device;

namespace RoomTemp.Models.GraphQL.Device
{
    public class DeviceMutation : ObjectGraphType
    {
        public DeviceMutation(IDeviceRepository deviceRepository)
        {
            Name = "Mutation";

            FieldAsync<DeviceType>(
                "createDevice",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<DeviceInputType>> { Name = "device" }
                ),
                resolve: async context =>
                {
                    var device = context.GetArgument<Data.Device>("device");
                    return await deviceRepository.CreateDevice(device);
                });

            FieldAsync<DeviceType>(
                "updateDevice",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                        { Name = "id", Description = "id of the device" },
                    new QueryArgument<NonNullGraphType<DeviceInputType>> { Name = "device" }
                ),
                resolve: async context =>
                {
                    var device = context.GetArgument<Data.Device>("device");
                    device.DeviceId = context.GetArgument<int>("id");
                    return await deviceRepository.UpdateDevice(device);
                });
        }
    }
}