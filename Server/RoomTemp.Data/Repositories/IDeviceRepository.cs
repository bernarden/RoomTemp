using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoomTemp.Data.Repositories
{
    public interface IDeviceRepository
    {
        Task<Device> Get(int id);
        
        Task<IEnumerable<Device>> GetAll();
        
        Task<Device> CreateDevice(Device device);
        
        Task<Device> UpdateDevice(Device device);
    }
}