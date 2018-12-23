using GraphQL.Types;
using RoomTemp.Data.Repositories;
using RoomTemp.Models.GraphQL.Device;

namespace RoomTemp.Models.GraphQL.Device
{
    public class DeviceMutation : ObjectGraphType
    {
        public DeviceMutation(DeviceRepository deviceRepository)
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
                    device = deviceRepository.Add(device);
                    await deviceRepository.SaveChangesAsync();
                    return device;
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
                    device.Id = context.GetArgument<int>("id");
                    device = deviceRepository.Update(device);
                    await deviceRepository.SaveChangesAsync();
                    return device;
                });
        }
    }
}