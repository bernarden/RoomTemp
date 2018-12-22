using GraphQL.Types;

namespace RoomTemp.Models.GraphQL.Device
{
    public class DeviceType : ObjectGraphType<Data.Device>
    {
        public DeviceType()
        {
            Field(x => x.DeviceId).Description("The Id of the Device.");
            Field(x => x.Name).Description("The name of the Device.");
        }
    }
}