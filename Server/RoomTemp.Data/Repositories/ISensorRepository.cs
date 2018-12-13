using System.Collections.Generic;
using System.Threading.Tasks;

namespace RoomTemp.Data.Repositories
{
    public interface ISensorRepository
    {
        Task<Sensor> Get(int id);

        Task<IEnumerable<Sensor>> GetAll();

        Task<Sensor> CreateSensor(Sensor sensor);

        Task<Sensor> UpdateSensor(Sensor sensor);
    }
}
