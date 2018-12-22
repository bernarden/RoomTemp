using GraphQL.Types;

namespace RoomTemp.Models.GraphQL.Device
{
    public class DeviceInputType : InputObjectGraphType<Data.Device>
    {
        public DeviceInputType()
        {
            Name = "DeviceInput";
            Field(x => x.Name);
        }
    }
}